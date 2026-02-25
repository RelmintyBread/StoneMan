using UnityEngine;

public class StoneManAI : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 1f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    [Header("Teleport")]
    public Transform[] teleportPoints;
    public float teleportCooldown = 2f;
    private float teleportTimer;

    private Rigidbody2D rb;

    private enum State { Patrol, Teleport, Chase }
    private State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
            player = foundPlayer.transform;

        currentState = State.Patrol;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Teleport:
                TeleportBehaviour();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    // ---------------- PATROL ----------------
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];

        MoveTo(targetPoint.position);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            currentPointIndex++;

            if (currentPointIndex >= patrolPoints.Length)
                currentPointIndex = 0;
        }
    }

    // ---------------- TELEPORT ----------------
    void TeleportBehaviour()
    {
        teleportTimer += Time.fixedDeltaTime;

        if (teleportTimer >= teleportCooldown)
        {
            teleportTimer = 0f;

            if (teleportPoints.Length == 0) return;

            int randomIndex = Random.Range(0, teleportPoints.Length);

            // instant teleport
            transform.position = teleportPoints[randomIndex].position;
        }
    }

    // ---------------- CHASE ----------------
    void Chase()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            MoveTo(player.position);
        }
    }

    // ---------------- MOVE FUNCTION ----------------
    void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // ---------------- TRIGGER DETECTION ----------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TeleportZone"))
        {
            currentState = State.Teleport;
        }

        if (other.CompareTag("ChaseZone"))
        {
            currentState = State.Chase;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TeleportZone"))
        {
            ReturnToNearestPatrol();
            currentState = State.Patrol;
        }

        if (other.CompareTag("ChaseZone"))
        {
            currentState = State.Teleport;
        }
    }

    // ---------------- FIND NEAREST PATROL ----------------
void ReturnToNearestPatrol()
{
    if (patrolPoints == null || patrolPoints.Length == 0)
        return;

    float shortestDistance = Mathf.Infinity;
    int nearestIndex = 0;

    for (int i = 0; i < patrolPoints.Length; i++)
    {
        if (patrolPoints[i] == null)
            continue; // skip destroyed points

        float distance = Vector2.Distance(
            transform.position,
            patrolPoints[i].position
        );

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            nearestIndex = i;
        }
    }

    currentPointIndex = nearestIndex;
}
}