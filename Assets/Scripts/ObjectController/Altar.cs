using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    [Header("Ending Cutscene UI")]
    public GameObject endingUIPanel;

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

            if (endingUIPanel != null)
            {
                endingUIPanel.SetActive(true); // Trigger ending cutscene UI
            }
        }
        else
        {
            Debug.Log("You need more artifacts!");
        }
    }
}