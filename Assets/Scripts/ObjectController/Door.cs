using UnityEngine;
using Pathfinding;

public class Door : MonoBehaviour, IInteractable, ISaveable
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

    [SerializeField] private string uniqueID;   // UNIQUE ID per pintu, untuk sistem save

    public bool isOpen = false;
    public bool IsOpen => isOpen;

    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        SaveManager.RegisterSaveable(this);
    }

    void Start()
    {
        // Pastikan layer awal sesuai dengan state pintu
        gameObject.layer = LayerMask.NameToLayer(isOpen ? openLayerName : closedLayerName);
    }

    // ===== Interaction Handlers =====
    public void ShowInteractUI() { }
    public void HideInteractUI() { }
    public void StartInteract() => Interact();
    public void StopInteract() { }

    public void Interact()
    {
        engsel.Rotate(0f, 0f, isOpen ? -90f : 90f);
        isOpen = !isOpen;
        AudioManager.Instance?.PlayDoorOpen();

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

    // ===== Save System Handlers =====
    public void OnSave(SaveData data)
    {
        if (isOpen && !data.isDoorOpen.Contains(uniqueID))
        {
            data.isDoorOpen.Add(uniqueID);
        }

        if (!isOpen && data.isDoorOpen.Contains(uniqueID))
        {
            data.isDoorOpen.Remove(uniqueID);
        }
    }

    public void OnLoad(SaveData data)
    {
        // if (data.doorStates != null && data.doorStates.TryGetValue(gameObject.name, out bool savedState))
        // {
        //     if (savedState != isOpen)
        //     {
        //         Interact(); // Toggle ke state yang benar
        //     }
        // }

        if (data.isDoorOpen.Contains(uniqueID) && !isOpen)
        {
            Interact(); // Buka pintu jika seharusnya terbuka tapi saat ini tertutup
        }
        else if (!data.isDoorOpen.Contains(uniqueID) && isOpen)
        {
            Interact(); // Tutup pintu jika seharusnya tertutup tapi saat ini terbuka
        }
    }
}
