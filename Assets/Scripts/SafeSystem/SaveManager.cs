using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private static readonly List<ISaveable> pendingRegistrations = new List<ISaveable>();

    private string saveFilePath;
    private readonly List<ISaveable> saveables = new List<ISaveable>();
    private SaveData pendingLoadData;

    // ===== Lifecycle =====
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureSavePath();
        FlushPendingRegistrations();
    }

    // ===== Public API (Main Flow) =====
    public static void RegisterSaveable(ISaveable obj)
    {
        if (obj == null)
        {
            return;
        }

        if (Instance != null)
        {
            Instance.RegisterInternal(obj);
            return;
        }

        if (!pendingRegistrations.Contains(obj))
        {
            pendingRegistrations.Add(obj);
        }
    }

    public void Save()
    {
        EnsureSavePath();

        SaveData data = new SaveData();
        data.EnsureDefaults();
        data.savedScene = SceneManager.GetActiveScene().name;

        CleanupDestroyedSaveables();
        LogEditor($"[SaveManager] Saving {saveables.Count} objects to: {saveFilePath}");

        foreach (ISaveable saveable in saveables)
        {
            saveable.OnSave(data);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    // Optional: load data ke scene yang sedang aktif (tanpa pindah scene).
    // Berguna untuk tombol debug/quick-load di dalam gameplay.
    [ContextMenu("Load Save Data (Current Scene)")]
    public bool Load()
    {
        EnsureSavePath();

        if (!TryReadSaveData(out SaveData data))
        {
            return false;
        }

        ApplyData(data);
        return true;
    }

    public bool LoadSavedGame()
    {
        EnsureSavePath();

        if (!TryReadSaveData(out SaveData data) || string.IsNullOrEmpty(data.savedScene))
        {
            return false;
        }

        // Bersihkan registry scene lama, agar saat scene baru selesai load
        // data di-apply ke objek saveable yang baru register.
        pendingLoadData = data;
        saveables.Clear();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(data.savedScene);
        return true;
    }

    public bool HasSave()
    {
        EnsureSavePath();
        return File.Exists(saveFilePath);
    }

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(saveFilePath);
        }
    }

    // Optional: dipakai jika UI mau menampilkan "Continue ke scene X".
    public string GetSavedSceneName()
    {
        if (!TryReadSaveData(out SaveData data) || string.IsNullOrEmpty(data.savedScene))
        {
            return string.Empty;
        }

        return data.savedScene;
    }

    // Optional: dipakai jika proses load dibatalkan di tengah jalan.
    public void ClearPendingLoadData()
    {
        pendingLoadData = null;
    }

    // ===== Private Helpers =====
    private void RegisterInternal(ISaveable obj)
    {
        if (obj == null)
        {
            return;
        }

        if (!saveables.Contains(obj))
        {
            saveables.Add(obj);
        }
    }

    private void ApplyData(SaveData data)
    {
        CleanupDestroyedSaveables();
        LogEditor($"[SaveManager] Loading into {saveables.Count} objects from: {saveFilePath}");

        foreach (ISaveable saveable in saveables)
        {
            saveable.OnLoad(data);
        }
    }

    private bool TryReadSaveData(out SaveData data)
    {
        data = null;

        if (!HasSave())
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            data.EnsureDefaults();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void CleanupDestroyedSaveables()
    {
        for (int i = saveables.Count - 1; i >= 0; i--)
        {
            if (saveables[i] == null)
            {
                saveables.RemoveAt(i);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (pendingLoadData == null)
        {
            return;
        }

        ApplyData(pendingLoadData);
        pendingLoadData = null;
        LogEditor("[SaveManager] Scene loaded and save data applied.");
    }

    private void EnsureSavePath()
    {
        if (string.IsNullOrEmpty(saveFilePath))
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
            LogEditor($"[SaveManager] Save path: {saveFilePath}");
        }
    }

    private void FlushPendingRegistrations()
    {
        for (int i = 0; i < pendingRegistrations.Count; i++)
        {
            RegisterInternal(pendingRegistrations[i]);
        }

        pendingRegistrations.Clear();
    }

    private static void LogEditor(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
}
