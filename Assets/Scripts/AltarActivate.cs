using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    public void ShowInteractUI()
    {
        // Only show UI if player collected all artifacts
        if (Artifact.collectedArtifacts >= Artifact.totalArtifactsRequired)
            UIHandler.Instance?.ShowInteractPrompt();
        else {
            UIHandler.Instance?.HideInteractPrompt();
        }

    }

    public void HideInteractUI()
    {
        UIHandler.Instance?.HideInteractPrompt();
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
        if (Artifact.collectedArtifacts >= Artifact.totalArtifactsRequired)
        {
            Debug.Log("Altar Cutscene Activated");
        }
        else
        {
            Debug.Log("You need more artifacts!");
        }
    }
}
