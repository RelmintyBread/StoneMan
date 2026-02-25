using UnityEngine;

public class StoneManAI : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaseDistance = 6f;
    public float stopDistance = 1f;

    [Header("Patrol")]
    public Transform[] patrolPoints;

    private int currentPointIndex = 0;
    private Rigidbody2D rb;

    private enum State { Patrol, Chase }
    private State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
            player = foundPlayer.transform;

        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseDistance)
            currentState = State.Chase;
        else
            currentState = State.Patrol;
    }

    void FixedUpdate()
    {
        if (currentState == State.Patrol)
            Patrol();
        else if (currentState == State.Chase)
            Chase();
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];

        MoveTo(targetPoint.position);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            currentPointIndex++;

            if (currentPointIndex >= patrolPoints.Length)
                currentPointIndex = 0; // loop back to start
        }
    }

    void Chase()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            MoveTo(player.position);
        }
    }

    void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - rb.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}