using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerInteract playerInteract;

    void Start()
    {
        playerInteract = GetComponent<PlayerInteract>();
    }

    void Update()
    {
        if (playerInteract == null) return;

        // Nanti, hanya blok ini yang diganti ke New Input System
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInteract.TriggerInteractPressed();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            playerInteract.TriggerInteractReleased();
        }
    }
}
