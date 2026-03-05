using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameScipt : MonoBehaviour
{
    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();   

        SceneManager.LoadScene(1);
    }

    public void LoadSavedGame()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            string sceneName = PlayerPrefs.GetString("SavedScene");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("No Save Found");
        }
    }

    public void Quit()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}