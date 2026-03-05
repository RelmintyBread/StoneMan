using UnityEngine;

public class StoneManAI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHide hide;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public Transform[] patrolPoints;

    [Header("Detection Distances")]
    public float bustedDistance = 1f;
    public float chaseDistance = 3f;
    public float teleportDistance = 8f;
    public float noiseDistance = 5f;

    [Header("World & Obstacles")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Collider2D worldBounds;

    [Header("Teleport")]
    [SerializeField] private float teleportCooldown = 5f;
    [SerializeField] private int maxRetry = 7;

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private enum State { Patrol, Teleport, Chase }

    private State currentState;
    private Rigidbody2D rb;
    private bool isFrozen;
    private int currentPatrolIndex;
    private float teleportTimer;
    private float distanceToPlayer;

    // ─────────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                hide = found.GetComponent<PlayerHide>();
            }
        }

        RefreshDistanceToPlayer();
        UpdateState();
    }

    void Update()
    {
        if (player == null) return;

        RefreshDistanceToPlayer();
        UpdateState();

        if (distanceToPlayer <= bustedDistance)
        {
            OnPlayerBusted();
        }
    }

    void FixedUpdate()
    {
        if (isFrozen) return;

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Teleport: TeleportBehaviour(); break;
            case State.Chase: Chase(); break;
        }
    }

    // ─────────────────────────────────────────────
    //  STATE MANAGEMENT
    // ─────────────────────────────────────────────

    void UpdateState()
    {
        State previousState = currentState;
        bool playerIsHiding = hide != null && hide.IsHiding();

        if (playerIsHiding)
        {
            currentState = distanceToPlayer <= teleportDistance
                ? State.Teleport
                : State.Patrol;
        }
        else if (distanceToPlayer <= chaseDistance)
            currentState = State.Chase;
        else if (distanceToPlayer <= teleportDistance)
            currentState = State.Teleport;
        else
            currentState = State.Patrol;

        // Snap ke patrol point terdekat saat baru masuk state Patrol
        if (currentState == State.Patrol && previousState != State.Patrol)
            ReturnToNearestPatrol();
    }

    // ─────────────────────────────────────────────
    //  PATROL
    // ─────────────────────────────────────────────

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        if (target == null) { AdvancePatrolIndex(); return; }

        MoveTo(target.position);

        if (Vector2.Distance(transform.position, target.position) < 0.5f)
            AdvancePatrolIndex();
    }

    void AdvancePatrolIndex()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // ─────────────────────────────────────────────
    //  TELEPORT
    // ─────────────────────────────────────────────

    void TeleportBehaviour()
    {
        teleportTimer += Time.fixedDeltaTime;
        if (teleportTimer < teleportCooldown) return;
        teleportTimer = 0f;

        // Choose ring based on whether player is within noise range
        bool inNoiseRange = distanceToPlayer <= noiseDistance;
        float minRing = inNoiseRange ? chaseDistance : noiseDistance;
        float maxRing = inNoiseRange ? noiseDistance : teleportDistance;

        //Ini kalau di noise maka play noise dan teleportnya di daerah noise, kalau gak ya teleportnya di luar noise tapi masih dalam teleportDistance. Jadi kalau player ngumpet tapi masih kedengeran, StoneMan bakal teleport lebih dekat ke player. Kalau player ngumpet dan gak kedengeran, StoneMan bakal teleport lebih jauh, mungkin buat cari-cari player.
        if (inNoiseRange)
            Debug.Log("StoneMan hears the player — teleporting close!");

        if (TryFindValidTeleportPos(minRing, maxRing, out Vector2 dest))
            transform.position = dest;
        else
            Debug.LogWarning("StoneMan: no valid teleport position found after max retries.");
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

    bool IsPositionValid(Vector2 pos)
    {
        bool insideWorld = worldBounds != null && worldBounds.OverlapPoint(pos);
        bool blocked = Physics2D.OverlapCircle(pos, 0.4f, obstacleLayer);
        return insideWorld && !blocked;
    }

    // ─────────────────────────────────────────────
    //  CHASE
    // ─────────────────────────────────────────────

    void Chase()
    {
        // State transition back to Teleport is handled by UpdateState();
        // Chase() only needs to move.
        MoveTo(player.position);
    }

    // ─────────────────────────────────────────────
    //  MOVEMENT
    // ─────────────────────────────────────────────

    void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    public void SetFrozen(bool frozen)
    {
        if (isFrozen == frozen) return;

        isFrozen = frozen;
        if (isFrozen)
            rb.linearVelocity = Vector2.zero;
    }

    // ─────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────

    void RefreshDistanceToPlayer()
    {
        if (player != null)
            distanceToPlayer = Vector2.Distance(transform.position, player.position);
    }

    void OnPlayerBusted()
    {
        Debug.Log("Player busted by StoneMan!");
        // TODO: trigger game-over / damage logic here
    }

    void ReturnToNearestPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        float shortest = Mathf.Infinity;
        int nearestIndex = 0;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            float d = Vector2.Distance(transform.position, patrolPoints[i].position);
            if (d < shortest)
            {
                shortest = d;
                nearestIndex = i;
            }
        }

        currentPatrolIndex = nearestIndex;
    }
}