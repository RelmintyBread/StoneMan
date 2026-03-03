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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
}
