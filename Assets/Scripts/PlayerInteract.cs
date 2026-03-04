using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public Vector2 currentFacingDirection = Vector2.up;

    private IInteractable currentInteractable; // Menyimpan target saat ini
    private bool isInteractPressed;
    public PlayerHide playerHide;

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
            detectedInteractable = hit.collider.GetComponentInParent<IInteractable>();
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
}
