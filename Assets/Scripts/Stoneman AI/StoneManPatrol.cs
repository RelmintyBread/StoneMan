using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // A* Pathfinding Project namespace

/// <summary>
/// Mengelola logika patrol Stoneman menggunakan A* Pathfinding Project.
/// Bertanggung jawab atas:
/// - Mengikuti patrol points secara berurutan
/// - Mencari patrol point terdekat yang BISA DICAPAI (by path length, bukan jarak lurus)
/// - Mengikuti waypoints hasil A* untuk menghindari obstacle
/// </summary>
[RequireComponent(typeof(Seeker))]
public class StoneManPatrol : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Pathfinding Settings")]
    [SerializeField] private float waypointReachedDistance = 0.4f;  // Jarak untuk dianggap sampai di waypoint
    [SerializeField] private float patrolPointReachedDistance = 0.5f; // Jarak untuk dianggap sampai di patrol point
    [SerializeField] private float pathRecalculateInterval = 0.5f;  // Seberapa sering path di-refresh (detik)

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private Seeker seeker;
    private StoneManMover mover;

    private List<Vector3> currentWaypoints = new List<Vector3>();
    private int waypointIndex = 0;
    private int currentPatrolIndex = 0;

    private bool isPatrolling = false;
    private bool isCalculatingPath = false;
    private float pathTimer = 0f;

    // ─────────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        mover = GetComponent<StoneManMover>();
    }

    void FixedUpdate()
    {
        if (!isPatrolling || isCalculatingPath) return;

        FollowPath();

        // Refresh path secara berkala agar tetap akurat
        pathTimer += Time.fixedDeltaTime;
        if (pathTimer >= pathRecalculateInterval)
        {
            pathTimer = 0f;
            RequestPathToCurrentTarget();
        }
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Mulai / lanjutkan patrol dari index saat ini.
    /// Dipanggil oleh StoneManAI setiap FixedUpdate saat state = Patrol.
    /// </summary>
    public void StartPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (isPatrolling) return; // Sudah berjalan, tidak perlu restart

        isPatrolling = true;
        RequestPathToCurrentTarget();
    }

    /// <summary>
    /// Hentikan patrol (dipanggil saat state berubah ke Chase/Teleport).
    /// </summary>
    public void StopPatrol()
    {
        isPatrolling = false;
        currentWaypoints.Clear();
    }

    /// <summary>
    /// Cari patrol point yang bisa dicapai dengan path terpendek dari posisi Stoneman saat ini.
    /// Menggunakan A* untuk mengukur panjang path — bukan jarak Euclidean.
    /// Dipanggil saat Stoneman baru masuk ke state Patrol dari state lain.
    /// </summary>
    public void ReturnToNearestReachable()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        StopPatrol();
        StartCoroutine(FindNearestReachablePatrolPoint());
    }

    // ─────────────────────────────────────────────
    //  PATH FOLLOWING
    // ─────────────────────────────────────────────

    void FollowPath()
    {
        if (currentWaypoints == null || currentWaypoints.Count == 0) return;
        if (waypointIndex >= currentWaypoints.Count)
        {
            OnReachedPatrolPoint();
            return;
        }

        Vector2 currentWaypoint = currentWaypoints[waypointIndex];
        mover.MoveTo(currentWaypoint);

        // Cek apakah sudah sampai di waypoint ini
        if (Vector2.Distance(transform.position, currentWaypoint) <= waypointReachedDistance)
        {
            waypointIndex++;

            // Cek apakah sudah sampai di patrol point tujuan
            Vector2 patrolTarget = patrolPoints[currentPatrolIndex].position;
            if (Vector2.Distance(transform.position, patrolTarget) <= patrolPointReachedDistance)
            {
                OnReachedPatrolPoint();
            }
        }
    }

    void OnReachedPatrolPoint()
    {
        // Maju ke patrol point berikutnya
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentWaypoints.Clear();
        waypointIndex = 0;
        RequestPathToCurrentTarget();
    }

    // ─────────────────────────────────────────────
    //  PATHFINDING — REQUEST
    // ─────────────────────────────────────────────

    void RequestPathToCurrentTarget()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (patrolPoints[currentPatrolIndex] == null) return;
        if (!seeker.IsDone()) return; // Tunggu request sebelumnya selesai

        Vector3 target = patrolPoints[currentPatrolIndex].position;
        seeker.StartPath(transform.position, target, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogWarning($"StoneManPatrol: Path error — {p.errorLog}");
            return;
        }

        currentWaypoints = p.vectorPath;
        waypointIndex = 0;
        isCalculatingPath = false;
    }

    // ─────────────────────────────────────────────
    //  PATHFINDING — NEAREST REACHABLE
    // ─────────────────────────────────────────────

    /// <summary>
    /// Loop semua patrol points, request path A* ke masing-masing,
    /// bandingkan panjang path, pilih yang terpendek dan valid.
    /// </summary>
    IEnumerator FindNearestReachablePatrolPoint()
    {
        isCalculatingPath = true;

        float shortestPathLength = Mathf.Infinity;
        int bestIndex = 0;
        bool foundAny = false;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            bool isDone = false;
            float length = Mathf.Infinity;
            bool isValid = false;

            // Request path ke patrol point ke-i
            seeker.StartPath(transform.position, patrolPoints[i].position, (Path p) =>
            {
                if (!p.error)
                {
                    length = CalculatePathLength(p);
                    isValid = true;
                }
                isDone = true;
            });

            // Tunggu path selesai dihitung sebelum lanjut ke titik berikutnya
            yield return new WaitUntil(() => isDone);

            if (isValid && length < shortestPathLength)
            {
                shortestPathLength = length;
                bestIndex = i;
                foundAny = true;
            }
        }

        if (foundAny)
        {
            currentPatrolIndex = bestIndex;
            Debug.Log($"StoneManPatrol: Returning to nearest reachable patrol point [{bestIndex}] (path length: {shortestPathLength:F1})");
        }
        else
        {
            Debug.LogWarning("StoneManPatrol: No reachable patrol point found! Defaulting to index 0.");
            currentPatrolIndex = 0;
        }

        isCalculatingPath = false;

        // Mulai patrol dari titik terbaik yang ditemukan
        isPatrolling = true;
        RequestPathToCurrentTarget();
    }

    /// <summary>
    /// Hitung total panjang path dari semua waypoints-nya.
    /// </summary>
    float CalculatePathLength(Path p)
    {
        float total = 0f;
        List<Vector3> waypoints = p.vectorPath;

        for (int i = 0; i < waypoints.Count - 1; i++)
            total += Vector3.Distance(waypoints[i], waypoints[i + 1]);

        return total;
    }
}