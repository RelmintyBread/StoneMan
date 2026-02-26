using UnityEngine;
public class FlashlightController : MonoBehaviour
{
    // ===== On Off =====
    private bool isHeld;
    private bool isOn;

    // ===== Battery =====
    [SerializeField] private float batteryLife = 100f;
    [SerializeField] private float batteryDrainRate = 5f;

    // ===== Flashlight Component =====
    [SerializeField] private Light flashlightLight;

    private float currentBattery;

    void Start()
    {
        currentBattery = batteryLife;
    }

    public void SetFlashlightHeld(bool held)
    {
        isHeld = held;
    }

    void Update()
    {
        HandleFlashlight();
    }

    void FixedUpdate()
    {
        HandleFreezeEnemy();
        HandleFacingLight();
    }

    void HandleFlashlight()
    {
        bool shouldBeOn = isHeld && HasBattery();

        if (shouldBeOn && !isOn)
        {
            TurnOn();
        }
        else if (!shouldBeOn && isOn)
        {
            TurnOff();
        }
    }

    bool HasBattery()
    {
        if (isOn)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                return false; // baterai habis
            }
        }

        return true; // nanti diganti logic battery
    }

    void TurnOn()
    {
        // enable light
        flashlightLight.enabled = true;
        isOn = true;
    }

    void TurnOff()
    {
        // disable light
        flashlightLight.enabled = false;
        isOn = false;
    }

    void HandleFreezeEnemy()
    {
        // nanti logic untuk freeze enemy
    }

    void HandleFacingLight()
    {
        // nanti logic untuk facing light
    }
}