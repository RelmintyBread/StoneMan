using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    [SerializeField] private Transform engsel;

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
            engsel.Rotate(0f, 0f, 90f);
        else
            engsel.Rotate(0f, 0f, -90f);

        isOpen = !isOpen;
    }
}