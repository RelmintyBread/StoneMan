using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public SaveData()
    {
        EnsureDefaults();
    }

    public void EnsureDefaults()
    {
        if (isDoorOpen == null) isDoorOpen = new List<string>();
        if (isBatteryCollected == null) isBatteryCollected = new List<string>();
        if (batteryPositions == null) batteryPositions = new List<int>();
        if (isArtifactCollected == null) isArtifactCollected = new List<string>();
        if (artifactPositions == null) artifactPositions = new List<int>();

        // Paper defaults
        if (isPaperCollected == null) isPaperCollected = new List<string>();
        if (paperPositions == null) paperPositions = new List<int>();

        if (savedScene == null) savedScene = string.Empty;
    }

    // Player-related data
    public Vector2 playerPosition;
    public float playerStamina;
    public string savedScene;

    // Door-related data
    public List<string> isDoorOpen;

    // Flashlight-related data
    public float flashlightBattery;

    // Battery-related data
    public List<string> isBatteryCollected;
    public List<int> batteryPositions;

    // Artifact-related data
    public List<string> isArtifactCollected;
    public List<int> artifactPositions;
    public int collectedArtifactsCount;
    public bool spawnGenerated;

    // Paper-related data
    public List<string> isPaperCollected;
    public List<int> paperPositions;

    // UI Guide-related data
    public bool isGuideDone;
    public bool isMovementDone;
    public bool isFlashlightDone;
    public bool isInteractDone;
    public bool isSaveDone;
}