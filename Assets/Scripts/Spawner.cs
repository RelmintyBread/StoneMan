using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject[] prefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int spawnAmount = 3;
    [SerializeField] private float spawnInterval = 30f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Radius")]
    [SerializeField] private Vector3 spawnCenterOffset = Vector3.zero;
    [SerializeField] private float minSpawnRadius = 0f;
    [SerializeField] private float maxSpawnRadius = 10f;
    [SerializeField] private float fixedY = 0f;

    [Header("Collision Check")]
    [SerializeField] private bool avoidOverlap = true;
    [SerializeField] private float overlapCheckRadius = 0.5f;
    [SerializeField] private LayerMask overlapMask = ~0;
    [SerializeField] private int maxPositionTry = 12;
    [SerializeField] private float nudgeDistance = 0.75f;

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }

        if (spawnInterval > 0f)
        {
            InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
        }
    }

    public void Spawn()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning($"[{nameof(Spawner)}] Prefabs belum diisi pada {name}.");
            return;
        }

        if (maxSpawnRadius < minSpawnRadius)
        {
            float temp = minSpawnRadius;
            minSpawnRadius = maxSpawnRadius;
            maxSpawnRadius = temp;
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
            Vector3 spawnPosition = GetValidSpawnPosition();
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();

        if (!avoidOverlap || overlapCheckRadius <= 0f)
        {
            return spawnPosition;
        }

        for (int i = 0; i < maxPositionTry; i++)
        {
            if (!HasBlockingObject(spawnPosition))
            {
                return spawnPosition;
            }

            Vector3 nudge = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized * nudgeDistance;

            spawnPosition += nudge;
            spawnPosition = ClampToSpawnRadius(spawnPosition);
        }

        return spawnPosition;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 center = transform.position + spawnCenterOffset;

        return new Vector3(
            center.x + randomCircle.x,
            fixedY,
            center.z + randomCircle.y
        );
    }

    private bool HasBlockingObject(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(
            position,
            overlapCheckRadius,
            overlapMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Transform hitTransform = hits[i].transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private Vector3 ClampToSpawnRadius(Vector3 position)
    {
        Vector3 center = transform.position + spawnCenterOffset;
        Vector3 offset = position - center;
        offset.y = 0f;

        float distance = offset.magnitude;

        if (distance > maxSpawnRadius && distance > 0f)
        {
            offset = offset.normalized * maxSpawnRadius;
        }
        else if (distance < minSpawnRadius && distance > 0f)
        {
            offset = offset.normalized * minSpawnRadius;
        }
        else if (distance == 0f && minSpawnRadius > 0f)
        {
            offset = Vector3.forward * minSpawnRadius;
        }

        return new Vector3(
            center.x + offset.x,
            fixedY,
            center.z + offset.z
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + spawnCenterOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, maxSpawnRadius);

        if (minSpawnRadius > 0f)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(center, minSpawnRadius);
        }
    }
}
