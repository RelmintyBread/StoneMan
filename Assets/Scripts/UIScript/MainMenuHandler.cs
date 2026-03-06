using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject mainMenuPanel;

    void Start()
    {
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
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

    public void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}