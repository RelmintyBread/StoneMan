using UnityEngine;

public class SavePoint2D : MonoBehaviour, IInteractable
{
    public void StartInteract()
    {
        // If you want save on press
        Interact();
    }

    public void StopInteract()
    {
        // Not needed for save
    }

    public void Interact()
    {
        SaveGame();
    }

    public void ShowInteractUI()
    {
        Debug.Log("Press E to Save");
        UIGameHandler.Instance?.ShowInteractPrompt();
    }

    public void HideInteractUI()
    {
        // Hide UI if you have one
        UIGameHandler.Instance?.HideInteractPrompt();
    }

    [ContextMenu("Test Save")]
    private void SaveGame()
    {
        SaveManager.Instance?.Save();

        Debug.Log("Game Saved!");
    }
}
