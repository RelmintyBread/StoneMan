using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 6f;
    public LayerMask interactableLayer;
    public Vector2 currentFacingDirection = Vector2.up;
    public PlayerHide playerHide;

    private IInteractable currentInteractable; // Menyimpan target saat ini
    private bool isInteractPressed;

    public static PlayerInteract Instance { get; private set; }

    void Awake()
    {
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
                if (isInteractPressed) currentInteractable.StopInteract();
                currentInteractable.HideInteractUI();
                currentInteractable = null;
                GuideManager.Instance?.NotifyInteractableDetected(null);
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
                if (isInteractPressed) currentInteractable.StopInteract();
                currentInteractable.HideInteractUI(); // Sembunyikan UI
            }

            currentInteractable = detectedInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.ShowInteractUI();
            }

            GuideManager.Instance?.NotifyInteractableDetected(currentInteractable);
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
            GuideManager.Instance?.NotifyInteractPressed(currentInteractable);
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
}
