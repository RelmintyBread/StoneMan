using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    public void ShowInteractUI()
    {
        UIGameHandler.Instance?.ShowInteractPrompt();
    }

    public void HideInteractUI()
    {
        UIGameHandler.Instance?.HideInteractPrompt();
    }

    public void StartInteract()
    {
        Interact();
    }

    public void StopInteract()
    {
    }

    public void Interact()
    {
        PlayerInteract playerInteract = PlayerInteract.Instance;
        if (playerInteract == null)
        {
            Debug.LogWarning("PlayerInteract not found.");
            return;
        }

        if (playerInteract.collectedArtifacts >= playerInteract.totalArtifactsRequired)
        {
            Debug.Log("Altar Cutscene Activated");
        }
        else
        {
            Debug.Log("You need more artifacts!");
        }
    }
}
