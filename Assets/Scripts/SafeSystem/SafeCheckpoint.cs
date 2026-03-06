using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    public void HideInteractUI()
    {
        // Hide UI if you have one
    }

    private void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        
        PlayerPrefs.Save();

        Debug.Log("Game Saved!");
    }
}