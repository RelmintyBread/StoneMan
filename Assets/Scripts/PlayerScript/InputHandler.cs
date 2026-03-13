using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerInteract playerInteract;

    private FlashlightController flashlightController;
    private float flashlightBlockedUntil;

    void Start()
    {
        playerInteract = GetComponent<PlayerInteract>();
        flashlightController = GetComponent<FlashlightController>();
    }

    void Update()
    {
        //Input for Interact
        if (playerInteract != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerInteract.TriggerInteractPressed();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                playerInteract.TriggerInteractReleased();
            }
        }

        //Input for Flashlight
        if (flashlightController != null)
        {
            if (Time.unscaledTime < flashlightBlockedUntil)
            {
                flashlightController.SetFlashlightHeld(false);
                return;
            }

            bool isHeld = Input.GetMouseButton(0);
            flashlightController.SetFlashlightHeld(isHeld);
            if (isHeld)
            {
                GuideManager.Instance?.NotifyFlashlightUsed();
            }
        }
    }

    public void BlockFlashlightForSeconds(float duration)
    {
        float until = Time.unscaledTime + Mathf.Max(0f, duration);
        if (until > flashlightBlockedUntil)
        {
            flashlightBlockedUntil = until;
        }
    }
}
