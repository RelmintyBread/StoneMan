using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

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

    private bool isInteractPromptVisible;

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
    }

    private void LateUpdate()
    {
        if (!isInteractPromptVisible || interactPrompt == null || playerTransform == null || worldCamera == null)
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
            return;
        }

        Vector3 worldPos = playerTransform.position + interactPromptWorldOffset;
        interactPrompt.position = worldCamera.WorldToScreenPoint(worldPos);
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
        artifactText.text = "Artefak: " + collectedArtifacts.ToString() + " / " + totalArtifactsRequired.ToString();
    }

    public void ShowInteractPrompt()
    {
        if (interactPrompt == null) return;

        isInteractPromptVisible = true;
        interactPrompt.gameObject.SetActive(true);
    }

    public void HideInteractPrompt()
    {
        if (interactPrompt == null) return;

        isInteractPromptVisible = false;
        interactPrompt.gameObject.SetActive(false);
    }
}
