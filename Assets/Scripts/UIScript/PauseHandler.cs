using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    public GameObject pausePanelUI;
    public GameObject gamePanelUI;

    void Start()
    {
        pausePanelUI.SetActive(false);
        gamePanelUI.SetActive(true);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Press ESC to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
                Pause();
            else
                Resume();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC PRESSED");
        }
    }

    // ===== BUTTON FUNCTIONS =====

    public void Resume()
    {
        pausePanelUI.SetActive(false);
        gamePanelUI.SetActive(true);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        // Clear pending load data to prevent old data from being applied on next load
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearPendingLoadData();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        pausePanelUI.SetActive(true);
        gamePanelUI.SetActive(false);
        Time.timeScale = 0f;
    }
}