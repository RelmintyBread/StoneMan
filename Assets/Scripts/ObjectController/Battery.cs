using UnityEngine;

public class Battery : MonoBehaviour, IInteractable
{
    [Header("Battery Settings")]
    [SerializeField] private string uniqueID;   // IMPORTANT: Set different ID per battery in Inspector
    [SerializeField] private int batteryAmount = 20;
    [SerializeField] private float requiredHoldTime = 1.5f;
    [SerializeField] private FlashlightController flashlightController;

    private float currentHoldTime = 0f;
    private bool isHolding = false;

    void Start()
    {
        // If battery already taken before → destroy immediately
        if (PlayerPrefs.GetInt("Battery_" + uniqueID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isHolding)
        {
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                Interact();
            }
        }
    }

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
    }

    public void StopInteract()
    {
        isHolding = false;
        currentHoldTime = 0f;
    }

    public void Interact()
    {
        isHolding = false;

        Debug.Log("Battery picked!");

        flashlightController.RechargeBattery(batteryAmount);

        // SAVE THAT THIS BATTERY IS TAKEN
        PlayerPrefs.SetInt("Battery_" + uniqueID, 1);
        PlayerPrefs.Save();

        Destroy(gameObject);
    }
}