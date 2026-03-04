using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    public void ShowInteractUI() { }
    public void HideInteractUI() { }

    public void StartInteract()
    {
        Interact();
    }

    public void StopInteract() { }

    public void Interact()
    {
        if (!isOpen)
            transform.Rotate(0f, 0f, 90f);
        else
            transform.Rotate(0f, 0f, -90f);

        isOpen = !isOpen;
    }
}