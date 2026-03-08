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
        ArtifactProgressManager progress = ArtifactProgressManager.Instance;
        if (progress == null)
        {
            Debug.LogWarning("ArtifactProgressManager not found.");
            return;
        }

        if (progress.HasRequiredArtifacts)
        {
            Debug.Log("Altar Cutscene Activated");
        }
        else
        {
            Debug.Log("You need more artifacts!");
        }
    }
}
