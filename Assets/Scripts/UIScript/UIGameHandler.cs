using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIGameHandler : MonoBehaviour
{
    public static UIGameHandler Instance { get; private set; }
    public bool IsGameOver { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Battery UI")]
    [SerializeField] private Image batteryUI;
    [SerializeField] private TextMeshProUGUI batteryText;
    private float batteryAmount;
    private float maxBatteryAmount;

    [Header("Stamina UI")]
    [SerializeField] private Image staminaUI;
    [SerializeField] private TextMeshProUGUI staminaText;
    private float staminaAmount;
    private float maxStaminaAmount;

    [Header("Artifact UI")]
    [SerializeField] private TextMeshProUGUI artifactText;
    private int collectedArtifacts;
    private int totalArtifactsRequired;

    [Header("Interact UI")]
    [SerializeField] private RectTransform interactPrompt;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Vector3 interactPromptWorldOffset = new Vector3(0f, 2f, 0f);

    [Header("Hold Interact UI")]
    [SerializeField] private GameObject interactProgressPanel;
    [SerializeField] private Image interactProgressFill;
    [SerializeField] private RectTransform interactProgressRect;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    private bool isInteractPromptVisible;
    private bool isHoldInteractionActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        if (interactPrompt != null)
        {
            interactPrompt.gameObject.SetActive(false);
        }

        if (interactProgressPanel != null)
        {
            interactProgressPanel.SetActive(false);
        }

        if (interactProgressRect == null && interactProgressPanel != null)
        {
            interactProgressRect = interactProgressPanel.GetComponent<RectTransform>();
        }

        if (interactProgressFill != null)
        {
            interactProgressFill.fillAmount = 0f;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        IsGameOver = false;
    }

    private void LateUpdate()
    {
        if (playerTransform == null || worldCamera == null)
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
            return;
        }

        Vector3 worldPos = playerTransform.position + interactPromptWorldOffset;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        if (isInteractPromptVisible && interactPrompt != null)
        {
            interactPrompt.position = screenPos;
        }

        if (isHoldInteractionActive && interactProgressRect != null)
        {
            interactProgressRect.position = screenPos;
        }
    }

    public void SetBatteryUI(float batteryAmount, float maxBatteryAmount)
    {
        this.batteryAmount = batteryAmount;
        this.maxBatteryAmount = maxBatteryAmount;
        batteryText.text = batteryAmount.ToString() + " / " + maxBatteryAmount.ToString();
        batteryUI.fillAmount = (float)batteryAmount / maxBatteryAmount;
    }

    public void SetStaminaUI(float staminaAmount, float maxStaminaAmount)
    {
        this.staminaAmount = staminaAmount;
        this.maxStaminaAmount = maxStaminaAmount;
        staminaText.text = staminaAmount.ToString() + " / " + maxStaminaAmount.ToString();
        staminaUI.fillAmount = staminaAmount / maxStaminaAmount;
    }

    public void SetArtifactUI(int collectedArtifacts, int totalArtifactsRequired)
    {
        this.collectedArtifacts = collectedArtifacts;
        this.totalArtifactsRequired = totalArtifactsRequired;
        artifactText.text = collectedArtifacts.ToString() + " / " + totalArtifactsRequired.ToString();
    }

    public void ShowInteractPrompt()
    {
        if (interactPrompt == null) return;
        if (isHoldInteractionActive) return;

        isInteractPromptVisible = true;
        interactPrompt.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        if (interactPrompt == null) return;

        isInteractPromptVisible = false;
        interactPrompt.gameObject.SetActive(false);
    }

    public void BeginHoldInteraction()
    {
        isHoldInteractionActive = true;
        HideInteractPrompt();

        if (interactProgressPanel != null)
        {
            interactProgressPanel.SetActive(true);
        }

        if (interactProgressFill != null)
        {
            interactProgressFill.fillAmount = 0f;
        }
    }

    public void UpdateHoldInteractionProgress(float progress01)
    {
        if (!isHoldInteractionActive) return;
        if (interactProgressFill == null) return;

        interactProgressFill.fillAmount = Mathf.Clamp01(progress01);
    }

    public void EndHoldInteraction(bool restorePrompt = true)
    {
        isHoldInteractionActive = false;

        if (interactProgressPanel != null)
        {
            interactProgressPanel.SetActive(false);
        }

        if (interactProgressFill != null)
        {
            interactProgressFill.fillAmount = 0f;
        }

        if (restorePrompt)
        {
            ShowInteractPrompt();
        }
    }

    public void ShowGameOverPanel()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        EndHoldInteraction(false);
        HideInteractPrompt();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartFromCheckpoint()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        if (SaveManager.Instance != null && SaveManager.Instance.LoadSavedGame())
        {
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        IsGameOver = false;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearPendingLoadData();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
