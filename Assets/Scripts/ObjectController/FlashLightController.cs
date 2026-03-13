using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
public class FlashlightController : MonoBehaviour, ISaveable
{
    // ===== On Off =====
    private bool isHeld;
    private bool isOn;

    // ===== Battery =====
    [Header("Battery Settings")]
    [SerializeField] private float batteryLife = 100f;
    [SerializeField] private float batteryDrainRate = 5f;

    // ===== Flashlight Component =====
    [Header("Flashlight Settings")]
    [SerializeField] private Light2D flashlightLight;
    [SerializeField] private float lightAngleOffset = -90f;

    // ===== Freeze Enemy Component =====
    [Header("Freeze Enemy")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float freezeRange = 8f;
    [SerializeField, Range(1f, 180f)] private float freezeConeAngle = 40f;
    [SerializeField] private bool checkObstacle = true;
    [SerializeField] private Vector2 lightForwardLocalAxis = Vector2.up;

    private float currentBattery;
    private readonly Collider2D[] enemyBuffer = new Collider2D[32];
    private readonly HashSet<StoneManAI> frozenEnemies = new HashSet<StoneManAI>();
    private readonly HashSet<StoneManAI> detectedEnemies = new HashSet<StoneManAI>();
    private readonly List<StoneManAI> releaseBuffer = new List<StoneManAI>();

    private UIGameHandler uiHandler;
    private bool hasLoadedData;

    void Awake()
    {
        SaveManager.RegisterSaveable(this);
    }

    void Start()
    {
        uiHandler = UIGameHandler.Instance;
        flashlightLight.enabled = false;
        if (!hasLoadedData)
        {
            currentBattery = batteryLife;
        }
        HandleUpdateUI();
    }

    public void SetFlashlightHeld(bool held)
    {
        isHeld = held;
    }

    void Update()
    {
        HandleFlashlight();
        HandleBattery();
        HandleUpdateUI();
        HandleFacingLight();
    }

    void FixedUpdate()
    {
        HandleFreezeEnemy();
    }

    void OnDisable()
    {
        ReleaseAllFrozenEnemies();
    }

    // ===== Flashlight Handlers =====
    void TurnOn()
    {
        // enable light
        flashlightLight.enabled = true;
        Debug.Log($"ON: enabled={flashlightLight.enabled}, pos={flashlightLight.transform.position}");
        isOn = true;
        AudioManager.Instance?.PlayFlashlightClick();
    }

    void TurnOff()
    {
        // disable light
        flashlightLight.enabled = false;
        isOn = false;
        AudioManager.Instance?.PlayFlashlightClick();
    }

    // ===== Main Handlers =====
    void HandleFlashlight()
    {
        if (IsPausedOrGameOver())
        {
            if (isOn)
            {
                TurnOff();
            }
            return;
        }

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

    void HandleUpdateUI()
    {
        if (uiHandler != null)
        {
            uiHandler.SetBatteryUI(currentBattery, batteryLife);
        }
    }

    void HandleFreezeEnemy()
    {
        if (!isOn || flashlightLight == null)
        {
            ReleaseAllFrozenEnemies();
            return;
        }

        Vector2 origin = flashlightLight.transform.position;
        Vector2 forward = flashlightLight.transform.TransformDirection(lightForwardLocalAxis).normalized;
        float halfCone = freezeConeAngle * 0.5f;

        detectedEnemies.Clear();
        ContactFilter2D enemyFilter = new ContactFilter2D();
        enemyFilter.useLayerMask = true;
        enemyFilter.layerMask = enemyLayer;
        enemyFilter.useTriggers = true;

        int count = Physics2D.OverlapCircle(origin, freezeRange, enemyFilter, enemyBuffer);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = enemyBuffer[i];
            if (hit == null) continue;

            StoneManAI enemy = hit.GetComponent<StoneManAI>();
            if (enemy == null) continue;

            Vector2 toEnemy = (Vector2)enemy.transform.position - origin;
            float distance = toEnemy.magnitude;
            if (distance <= Mathf.Epsilon) continue;

            float angle = Vector2.Angle(forward, toEnemy / distance);
            if (angle > halfCone) continue;

            if (checkObstacle && Physics2D.Linecast(origin, enemy.transform.position, obstacleLayer))
            {
                continue;
            }

            detectedEnemies.Add(enemy);
            if (!frozenEnemies.Contains(enemy))
            {
                enemy.SetFrozen(true);
                AudioManager.Instance?.PlayStun();
                frozenEnemies.Add(enemy);
            }
        }

        releaseBuffer.Clear();
        foreach (StoneManAI enemy in frozenEnemies)
        {
            if (enemy == null || !detectedEnemies.Contains(enemy))
            {
                releaseBuffer.Add(enemy);
            }
        }

        for (int i = 0; i < releaseBuffer.Count; i++)
        {
            StoneManAI enemy = releaseBuffer[i];
            if (enemy != null)
            {
                enemy.SetFrozen(false);
            }

            frozenEnemies.Remove(enemy);
        }
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

    // ===== ISaveable Implementation =====
    public void OnSave(SaveData data)
    {
        // Save current battery level
        data.flashlightBattery = currentBattery;
    }

    public void OnLoad(SaveData data)
    {
        // Load current battery level
        currentBattery = data.flashlightBattery;
        hasLoadedData = true;
    }

    // ===== Helper Methods =====
    void ReleaseAllFrozenEnemies()
    {
        if (frozenEnemies.Count == 0) return;

        foreach (StoneManAI enemy in frozenEnemies)
        {
            if (enemy != null)
            {
                enemy.SetFrozen(false);
            }
        }

        frozenEnemies.Clear();
    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > batteryLife)
        {
            currentBattery = batteryLife;
        }
    }

    bool HasBattery()
    {
        return currentBattery > 0;
    }

    bool IsPausedOrGameOver()
    {
        if (Time.timeScale == 0f) return true;
        if (UIGameHandler.Instance != null && UIGameHandler.Instance.IsGameOver) return true;
        return PaperScript.GameState.IsPaused;
    }

}
