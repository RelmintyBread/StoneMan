using UnityEngine;
using System.Collections.Generic;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager Instance;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Scene Objects")]
    public Artifact[] artifacts;
    public Battery[] batteries;

    void Awake()
    {
        Instance = this;

        if (!PlayerPrefs.HasKey("SpawnGenerated"))
        {
            GenerateSpawn();
        }

        AssignPositions();
    }

    void GenerateSpawn()
    {
        if (spawnPoints.Length < artifacts.Length + batteries.Length)
        {
            Debug.LogError("Not enough spawn points!");
            return;
        }

        List<int> indexes = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
            indexes.Add(i);

        Shuffle(indexes);

        int counter = 0;

        // Assign artifact spawn indices
        for (int i = 0; i < artifacts.Length; i++)
        {
            PlayerPrefs.SetInt("ArtifactSpawn_" + i, indexes[counter]);
            counter++;
        }

        // Assign battery spawn indices
        for (int i = 0; i < batteries.Length; i++)
        {
            PlayerPrefs.SetInt("BatterySpawn_" + i, indexes[counter]);
            counter++;
        }

        PlayerPrefs.SetInt("SpawnGenerated", 1);
        PlayerPrefs.Save();
    }

    void AssignPositions()
    {
        // Move artifacts
        for (int i = 0; i < artifacts.Length; i++)
        {
            int index = PlayerPrefs.GetInt("ArtifactSpawn_" + i);
            artifacts[i].transform.position = spawnPoints[index].position;
        }

        // Move batteries
        for (int i = 0; i < batteries.Length; i++)
        {
            int index = PlayerPrefs.GetInt("BatterySpawn_" + i);
            batteries[i].transform.position = spawnPoints[index].position;
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
}