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
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmMainMenu);
    }
    public void StartNewGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        SceneManager.LoadScene(1);

    }

    public void LoadSavedGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (SaveManager.Instance != null && SaveManager.Instance.LoadSavedGame())
        {
            return;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager instance not found. Cannot load saved game.");
        }

        StartNewGame();
        Debug.Log("No Save Found");
    }

    public void Quit()
    {
        AudioManager.Instance?.PlayButtonClick();
        Debug.Log("Quit Game");

        Application.Quit();
    }

    public void OpenOptionPanel()
    {
        AudioManager.Instance?.PlayButtonClick();
        optionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseOptionPanel()
    {
        AudioManager.Instance?.PlayButtonClick();
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
