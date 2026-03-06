using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    public void ShowInteractUI()
    {
        UIHandler.Instance?.ShowInteractPrompt();
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
