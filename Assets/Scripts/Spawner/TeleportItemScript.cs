using UnityEngine;
using System.Collections.Generic;

public class ItemSpawnManager : MonoBehaviour, ISaveable
{
    public static ItemSpawnManager Instance;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Scene Objects")]
    public Artifact[] artifacts;
    public Battery[] batteries;
    public PaperScript[] papers;

    private readonly List<int> artifactSpawnIndexes = new List<int>();
    private readonly List<int> batterySpawnIndexes = new List<int>();
    private readonly List<int> paperSpawnIndexes = new List<int>();

    private bool spawnGenerated;

    void Awake()
    {
        SaveManager.RegisterSaveable(this);
        Instance = this;
    }

    private void Start()
    {
        if (!spawnGenerated)
        {
            GenerateSpawn();
        }

        AssignPositions();
    }

    void GenerateSpawn()
    {
        if (spawnPoints.Length < artifacts.Length + batteries.Length + papers.Length)
        {
            Debug.LogError("Not enough spawn points!");
            return;
        }

        artifactSpawnIndexes.Clear();
        batterySpawnIndexes.Clear();
        paperSpawnIndexes.Clear();

        List<int> indexes = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            indexes.Add(i);
        }

        Shuffle(indexes);

        int counter = 0;

        // Artifact positions
        for (int i = 0; i < artifacts.Length; i++)
        {
            artifactSpawnIndexes.Add(indexes[counter]);
            counter++;
        }

        // Battery positions
        for (int i = 0; i < batteries.Length; i++)
        {
            batterySpawnIndexes.Add(indexes[counter]);
            counter++;
        }

        // Paper positions
        for (int i = 0; i < papers.Length; i++)
        {
            paperSpawnIndexes.Add(indexes[counter]);
            counter++;
        }

        spawnGenerated = true;
    }

    void AssignPositions()
    {
        if (artifactSpawnIndexes.Count != artifacts.Length ||
            batterySpawnIndexes.Count != batteries.Length ||
            paperSpawnIndexes.Count != papers.Length)
        {
            return;
        }

        // Artifact
        for (int i = 0; i < artifacts.Length; i++)
        {
            int index = artifactSpawnIndexes[i];
            if (index < 0 || index >= spawnPoints.Length) continue;
            artifacts[i].transform.position = spawnPoints[index].position;
        }

        // Battery
        for (int i = 0; i < batteries.Length; i++)
        {
            int index = batterySpawnIndexes[i];
            if (index < 0 || index >= spawnPoints.Length) continue;
            batteries[i].transform.position = spawnPoints[index].position;
        }

        // Paper
        for (int i = 0; i < papers.Length; i++)
        {
            int index = paperSpawnIndexes[i];
            if (index < 0 || index >= spawnPoints.Length) continue;
            papers[i].transform.position = spawnPoints[index].position;
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    // ===== Save System Handlers =====

    public void OnSave(SaveData data)
    {
        data.spawnGenerated = spawnGenerated;

        data.artifactPositions.Clear();
        data.batteryPositions.Clear();
        data.paperPositions.Clear();

        for (int i = 0; i < artifactSpawnIndexes.Count; i++)
        {
            data.artifactPositions.Add(artifactSpawnIndexes[i]);
        }

        for (int i = 0; i < batterySpawnIndexes.Count; i++)
        {
            data.batteryPositions.Add(batterySpawnIndexes[i]);
        }

        for (int i = 0; i < paperSpawnIndexes.Count; i++)
        {
            data.paperPositions.Add(paperSpawnIndexes[i]);
        }
    }

    public void OnLoad(SaveData data)
    {
        artifactSpawnIndexes.Clear();
        batterySpawnIndexes.Clear();
        paperSpawnIndexes.Clear();

        if (!data.spawnGenerated)
        {
            spawnGenerated = false;
            GenerateSpawn();
            AssignPositions();
            return;
        }

        spawnGenerated = true;

        for (int i = 0; i < data.artifactPositions.Count; i++)
        {
            artifactSpawnIndexes.Add(data.artifactPositions[i]);
        }

        for (int i = 0; i < data.batteryPositions.Count; i++)
        {
            batterySpawnIndexes.Add(data.batteryPositions[i]);
        }

        for (int i = 0; i < data.paperPositions.Count; i++)
        {
            paperSpawnIndexes.Add(data.paperPositions[i]);
        }

        AssignPositions();
    }
}