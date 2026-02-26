using UnityEngine;
using UnityEngine.Rendering.Universal;
public class FlashlightController : MonoBehaviour
{
    // ===== On Off =====
    private bool isHeld;
    private bool isOn;

    // ===== Battery =====
    [SerializeField] private float batteryLife = 100f;
    [SerializeField] private float batteryDrainRate = 5f;

    // ===== Flashlight Component =====
    [SerializeField] private Light2D flashlightLight;
    [SerializeField] private float lightAngleOffset = -90f;

    private float currentBattery;

    void Start()
    {
        flashlightLight.enabled = false;
        currentBattery = batteryLife;
    }

    public void SetFlashlightHeld(bool held)
    {
        isHeld = held;
    }

    void Update()
    {
        HandleFlashlight();
        HandleBattery();
        HandleFacingLight();
    }

    void FixedUpdate()
    {
        HandleFreezeEnemy();
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
        return currentBattery > 0;
    }

    void TurnOn()
    {
        // enable light
        flashlightLight.enabled = true;
        Debug.Log($"ON: enabled={flashlightLight.enabled}, pos={flashlightLight.transform.position}");
        isOn = true;
    }

    void TurnOff()
    {
        // disable light
        flashlightLight.enabled = false;
        isOn = false;
    }
    void HandleBattery()
    {
        if (isOn)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                TurnOff();
                Debug.Log("Battery depleted!");
            }
        }
    }

    void HandleFreezeEnemy()
    {
        // nanti logic untuk freeze enemy
    }

    void HandleFacingLight()
    {
        if (!isOn || flashlightLight == null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // ambil posisi mouse dalam world space
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = flashlightLight.transform.position.z;

        // Hitung arah dari flashlight ke mouse
        Vector2 direction = (mouseWorldPos - flashlightLight.transform.position).normalized;

        // Hitung sudut (dalam derajat)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotasi di sumbu Z (karena 2D)
        flashlightLight.transform.rotation = Quaternion.Euler(0f, 0f, angle + lightAngleOffset);

    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > batteryLife)
        {
            currentBattery = batteryLife;
        }
    }
}
