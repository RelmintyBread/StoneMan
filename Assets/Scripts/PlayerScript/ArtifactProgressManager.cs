using UnityEngine;

public class ArtifactProgressManager : MonoBehaviour, ISaveable
{
    public static ArtifactProgressManager Instance { get; private set; }

    [SerializeField] private int totalArtifactsRequired = 5;
    private int collectedArtifacts;

    public int CollectedArtifacts => collectedArtifacts;
    public int TotalArtifactsRequired => totalArtifactsRequired;
    public bool HasRequiredArtifacts => collectedArtifacts >= totalArtifactsRequired;

    private void Awake()
    {
        SaveManager.RegisterSaveable(this);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshArtifactUI();
    }

    public void CollectArtifact()
    {
        collectedArtifacts++;
        RefreshArtifactUI();
        CutsceneManager.Instance?.TryPlayCutscene(collectedArtifacts);
    }

    public void RefreshArtifactUI()
    {
        UIGameHandler.Instance?.SetArtifactUI(collectedArtifacts, totalArtifactsRequired);
    }

    public void OnSave(SaveData data)
    {
        data.collectedArtifactsCount = collectedArtifacts;
    }

    public void OnLoad(SaveData data)
    {
        collectedArtifacts = Mathf.Max(0, data.collectedArtifactsCount);
        RefreshArtifactUI();
    }
}
