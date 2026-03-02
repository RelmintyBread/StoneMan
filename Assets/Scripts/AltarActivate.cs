using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;

    public void ShowInteractUI()
    {
        if (interactUI == null) return;

        // Only show UI if player collected all artifacts
        if (Artifact.collectedArtifacts >= Artifact.totalArtifactsRequired)
            interactUI.SetActive(true);
        else {
            interactUI.SetActive(false);
        }

    }

    public void HideInteractUI()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
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