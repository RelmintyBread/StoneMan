using UnityEngine;
using Pathfinding;

public class Door : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform engsel;

    [Header("Layers")]
    [Tooltip("Layer saat pintu tertutup — diabaikan A*, dideteksi raycast Stoneman.")]
    [SerializeField] private string closedLayerName = "Door";
    [Tooltip("Layer saat pintu terbuka — dikenali A* sebagai obstacle.")]
    [SerializeField] private string openLayerName = "Obstacle";

    [Header("A* Graph Update")]
    [Tooltip("Radius area yang di-rescan A* setelah pintu bergerak. Sesuaikan dengan ukuran pintu.")]
    [SerializeField] private float graphUpdateRadius = 1.5f;

    public bool isOpen = false;
    private Collider2D doorCollider;

    public bool IsOpen => isOpen;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    public void ShowInteractUI() { }
    public void HideInteractUI() { }
    public void StartInteract() => Interact();
    public void StopInteract() { }

    public void Interact()
    {
        engsel.Rotate(0f, 0f, isOpen ? -90f : 90f);
        isOpen = !isOpen;

        // Swap layer: terbuka → jadi Obstacle (A* tahu), tertutup → jadi Door (A* abaikan)
        gameObject.layer = LayerMask.NameToLayer(isOpen ? openLayerName : closedLayerName);

        // Rescan area sekitar pintu agar A* tahu posisi collider yang baru
        UpdateGraphAroundDoor();
    }

    /// <summary>
    /// Rescan area lokal sekitar pintu saja — tidak perlu scan seluruh map.
    /// Collider sudah di layer Obstacle saat ini, jadi A* akan mengenalinya.
    /// </summary>
    void UpdateGraphAroundDoor()
    {
        var bounds = new Bounds(transform.position, Vector3.one * graphUpdateRadius * 2f);
        AstarPath.active.UpdateGraphs(bounds);
    }
}