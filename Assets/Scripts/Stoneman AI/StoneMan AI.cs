using UnityEngine;

/// <summary>
/// Orchestrator utama AI Stoneman.
/// Hanya bertanggung jawab atas:
/// - Deteksi jarak ke player
/// - State machine (Patrol / Teleport / Chase)
/// - Mendelegasikan eksekusi ke StoneManMover dan StoneManPatrol
/// </summary>
[RequireComponent(typeof(StoneManMover))]
[RequireComponent(typeof(StoneManPatrol))]
public class StoneManAI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHide hide;

    [Header("Detection Distances")]
    public float bustedDistance = 1.5f;
    public float chaseDistance = 3f;
    public float teleportDistance = 8f;
    public float noiseDistance = 5f;

    [Header("World & Obstacles")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Collider2D worldBounds;

    [Header("Teleport")]
    public float teleportCooldown = 2f;
    [SerializeField] private int maxRetry = 7;

    [Header("Anti Stuck System")]
    [SerializeField] private float stuckCheckInterval = 1.5f;
    [SerializeField] private float stuckTeleportRadius = 5f;

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private enum State { Patrol, Teleport, Chase }

    private State currentState;
    private State previousState;

    private StoneManMover mover;
    private StoneManPatrol patrol;
    private PlayerLife playerLife;

    private float distanceToPlayer;
    private float teleportTimer;
    private bool hasBustedPlayer;

    // Anti stuck
    private Vector2 lastPosition;
    private float stuckCheckTimer;

    // ─────────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    void Start()
    {
        mover = GetComponent<StoneManMover>();
        patrol = GetComponent<StoneManPatrol>();

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                hide = found.GetComponent<PlayerHide>();
                playerLife = found.GetComponent<PlayerLife>();
            }
        }
        else
        {
            playerLife = player.GetComponent<PlayerLife>();
        }

        lastPosition = transform.position;

        RefreshDistance();
        previousState = State.Patrol;
        UpdateState();
        hasBustedPlayer = false;
    }

    void Update()
    {
        if (player == null) return;

        RefreshDistance();
        UpdateState();

        if (!hasBustedPlayer && distanceToPlayer <= bustedDistance)
            OnPlayerBusted();

        CheckIfStuck();
    }

    void FixedUpdate()
    {
        if (mover.IsFrozen) return;

        switch (currentState)
        {
            case State.Patrol:
                patrol.StartPatrol();
                break;

            case State.Teleport:
                patrol.StopPatrol();
                TeleportBehaviour();
                break;

            case State.Chase:
                patrol.StopPatrol();
                AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmChase);
                mover.MoveTo(player.position);
                break;
        }
    }

    // ─────────────────────────────────────────────
    //  STATE MANAGEMENT
    // ─────────────────────────────────────────────

    void UpdateState()
    {
        previousState = currentState;
        bool playerIsHiding = hide != null && hide.IsHiding();

        if (playerIsHiding)
        {
            currentState = State.Patrol;
        }
        else if (distanceToPlayer <= chaseDistance)
            currentState = State.Chase;
        else if (distanceToPlayer <= teleportDistance)
            currentState = State.Teleport;
        else
            currentState = State.Patrol;

        if (currentState == State.Patrol && previousState != State.Patrol)
            patrol.ReturnToNearestReachable();

        if (currentState != State.Chase && previousState == State.Chase)
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmGameplay);

        if (distanceToPlayer > noiseDistance)
            AudioManager.Instance?.StopStoneman();
    }

    // ─────────────────────────────────────────────
    //  TELEPORT SYSTEM
    // ─────────────────────────────────────────────

    void TeleportBehaviour()
    {
        if (mover.IsFrozen) return;

        teleportTimer += Time.fixedDeltaTime;

        if (teleportTimer < teleportCooldown) return;

        teleportTimer = 0f;

        bool inNoiseRange = distanceToPlayer <= noiseDistance;
        float minRing = inNoiseRange ? chaseDistance : noiseDistance;
        float maxRing = inNoiseRange ? noiseDistance : teleportDistance;

        if (inNoiseRange)
        {
            Debug.Log("StoneMan hears the player — teleporting close!");
            AudioManager.Instance?.PlayStonemanStep();
        }

        if (TryFindValidTeleportPos(minRing, maxRing, out Vector2 dest))
        {
            transform.position = dest;
            mover.NotifyTeleported();
        }
        else
        {
            Debug.LogWarning("StoneMan: no valid teleport position found after max retries.");
        }
    }

    bool TryFindValidTeleportPos(float minDist, float maxDist, out Vector2 result)
    {
        for (int i = 0; i < maxRetry; i++)
        {
            Vector2 candidate = GenerateRingPosition(minDist, maxDist);

            if (IsPositionValid(candidate))
            {
                result = candidate;
                return true;
            }
        }

        result = Vector2.zero;
        return false;
    }

    Vector2 GenerateRingPosition(float minDistance, float maxDistance)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(minDistance, maxDistance);
        return (Vector2)player.position + dir * dist;
    }

    // Teleport normal (cek obstacle)
    bool IsPositionValid(Vector2 pos)
    {
        bool insideWorld = worldBounds != null && worldBounds.OverlapPoint(pos);
        bool blocked = Physics2D.OverlapCircle(pos, 0.4f, obstacleLayer);
        return insideWorld && !blocked;
    }

    // Teleport paksa (abaikan obstacle)
    bool IsPositionValidIgnoreObstacle(Vector2 pos)
    {
        bool insideWorld = worldBounds != null && worldBounds.OverlapPoint(pos);
        return insideWorld;
    }

    // ─────────────────────────────────────────────
    //  ANTI STUCK SYSTEM
    // ─────────────────────────────────────────────

void CheckIfStuck()
{
    stuckCheckTimer += Time.deltaTime;

    if (stuckCheckTimer < stuckCheckInterval) return;

    stuckCheckTimer = 0f;

    Vector2 currentPos = transform.position;

    float movement = Vector2.Distance(currentPos, lastPosition);

    if (movement < 0.2f)
    {
        Debug.Log("StoneMan is stuck! Forcing warp forward!");

        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        // warp di depan stoneman
        Vector2 newPos = currentPos + dirToPlayer * stuckTeleportRadius;

        if (IsPositionValidIgnoreObstacle(newPos))
        {
            transform.position = newPos;
            mover.NotifyTeleported();
        }
    }

    lastPosition = currentPos;
}

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    void RefreshDistance()
    {
        if (player != null)
            distanceToPlayer = Vector2.Distance(transform.position, player.position);
    }

    void OnPlayerBusted()
    {
        if (hasBustedPlayer) return;

        hasBustedPlayer = true;

        Debug.Log("Player busted by StoneMan!");

        patrol.StopPatrol();
        mover.SetFrozen(true);

        if (playerLife != null)
        {
            playerLife.Die();
            return;
        }

        UIGameHandler.Instance?.ShowGameOverPanel();
    }

    /// <summary>
    /// Freeze / unfreeze dari luar (misal cutscene).
    /// </summary>
    public void SetFrozen(bool frozen)
    {
        mover.SetFrozen(frozen);
    }
}