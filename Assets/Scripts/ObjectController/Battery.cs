using UnityEngine;

public class Battery : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Battery Settings")]
    [SerializeField] private string uniqueID;   // IMPORTANT: Set different ID per battery in Inspector
    [SerializeField] private int batteryAmount = 20;
    [SerializeField] private float requiredHoldTime = 1.5f;
    [SerializeField] private FlashlightController flashlightController;

    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool isCollected = false;

    void Awake()
    {
        SaveManager.RegisterSaveable(this);
    }

    void Update()
    {
        if (isHolding)
        {
            currentHoldTime += Time.deltaTime;
            UIGameHandler.Instance?.UpdateHoldInteractionProgress(currentHoldTime / requiredHoldTime);

            if (currentHoldTime >= requiredHoldTime)
            {
                Interact();
            }
        }
    }

    // ===== Interaction Handlers =====
    public void ShowInteractUI()
    {
        UIGameHandler.Instance?.ShowInteractPrompt();
    }

    public void HideInteractUI()
    {
        UIGameHandler.Instance?.HideInteractPrompt();
    }

    public void StartInteract()
    {
        isHolding = true;
        currentHoldTime = 0f;
        UIGameHandler.Instance?.BeginHoldInteraction();
    }

    public void StopInteract()
    {
        isHolding = false;
        currentHoldTime = 0f;
        UIGameHandler.Instance?.EndHoldInteraction();
    }

    public void Interact()
    {
        isHolding = false;
        UIGameHandler.Instance?.EndHoldInteraction(false);

        Debug.Log("Battery picked!");

        flashlightController.RechargeBattery(batteryAmount);
        isCollected = true;

        gameObject.SetActive(false);
    }

    // ===== Saveable Implementation =====
    public void OnSave(SaveData data)
    {
        if (isCollected && !data.isBatteryCollected.Contains(uniqueID))
        {
            data.isBatteryCollected.Add(uniqueID);
        }
    }

    public void OnLoad(SaveData data)
    {
        if (data.isBatteryCollected.Contains(uniqueID))
        {
            isCollected = true;
            gameObject.SetActive(false);
        }
    }
}
