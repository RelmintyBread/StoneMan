using UnityEngine;

public class PlayerInteract : MonoBehaviour, ISaveable
{
    public float interactDistance = 6f;
    public LayerMask interactableLayer;
    public Vector2 currentFacingDirection = Vector2.up;
    public PlayerHide playerHide;

    public int collectedArtifacts = 0; // Track jumlah artifact yang dikumpulkan
    public int totalArtifactsRequired = 5; // Total artifact yang dibutuhkan untuk menang

    private IInteractable currentInteractable; // Menyimpan target saat ini
    private bool isInteractPressed;

    public static PlayerInteract Instance { get; private set; }

    void Awake()
    {
        SaveManager.RegisterSaveable(this);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        playerHide = GetComponent<PlayerHide>();
    }

    void Update()
    {
        // Saat hidden, interaksi dipusatkan ke current hiding spot dan tidak perlu retarget raycast.
        if (playerHide != null && playerHide.IsHidden)
        {
            if (currentInteractable != null)
            {
                currentInteractable.HideInteractUI();
                if (isInteractPressed) currentInteractable.StopInteract();
                currentInteractable = null;
            }
            return;
        }

        // Lakukan Raycast setiap frame untuk mendeteksi objek
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentFacingDirection, interactDistance, interactableLayer);
        IInteractable detectedInteractable = null;

        if (hit.collider != null)
        {
            detectedInteractable = hit.collider.GetComponent<IInteractable>();
        }

        // Tangani perpindahan target, termasuk saat raycast kena collider non-interactable.
        if (detectedInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.HideInteractUI(); // Sembunyikan UI
                if (isInteractPressed) currentInteractable.StopInteract();
            }

            currentInteractable = detectedInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.ShowInteractUI();
            }
        }
    }

    public void SetFacingDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0f)
        {
            currentFacingDirection = direction.normalized;
        }
    }

    public void TriggerInteractPressed()
    {
        if (playerHide != null && playerHide.IsHidden)
        {
            playerHide.TryExitCurrentHidingSpot();
            return;
        }

        isInteractPressed = true;

        if (currentInteractable != null)
        {
            currentInteractable.StartInteract();
        }
    }

    public void TriggerInteractReleased()
    {
        if (!isInteractPressed) return;

        isInteractPressed = false;

        if (currentInteractable != null)
        {
            currentInteractable.StopInteract();
        }
    }

    // ===== Save System Handlers =====
    public void OnSave(SaveData data)
    {
        data.collectedArtifactsCount = collectedArtifacts;
    }

    public void OnLoad(SaveData data)
    {
        collectedArtifacts = data.collectedArtifactsCount;
    }
}
