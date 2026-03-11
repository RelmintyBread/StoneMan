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
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        SceneManager.LoadScene(1);

    }

    public void LoadSavedGame()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.LoadSavedGame())
        {
            return;
        }

        Debug.Log("No Save Found");
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
