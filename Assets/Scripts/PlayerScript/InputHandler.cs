using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerInteract playerInteract;

    private FlashlightController flashlightController;

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
            bool isHeld = Input.GetMouseButton(0);
            flashlightController.SetFlashlightHeld(isHeld);
            if (isHeld)
            {
                GuideManager.Instance?.NotifyFlashlightUsed();
            }
        }
    }
}
