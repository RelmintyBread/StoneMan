using UnityEngine;

public class Artifact : MonoBehaviour, IInteractable, ISaveable
{
    // Unique identifier for this artifact
    [SerializeField] private string artifactID;   // UNIQUE ID per artifact

    // Interaction variables
    [SerializeField] private float requiredHoldTime = 3f;
    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool isCollected = false;

    private void Awake()
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

        if (isCollected)
        {
            return;
        }

        ArtifactProgressManager progress = ArtifactProgressManager.Instance;
        if (progress == null)
        {
            Debug.LogWarning("ArtifactProgressManager not found.");
            return;
        }

        progress.CollectArtifact();
        isCollected = true;

        Debug.Log("Artifact collected! Total: " + progress.CollectedArtifacts + "/" + progress.TotalArtifactsRequired);

        gameObject.SetActive(false);
    }

    // ===== Save System Handlers =====
    public void OnSave(SaveData data)
    {
        if (isCollected && !data.isArtifactCollected.Contains(artifactID))
        {
            data.isArtifactCollected.Add(artifactID);
        }
    }
    public void OnLoad(SaveData data)
    {
        if (data.isArtifactCollected.Contains(artifactID))
        {
            isCollected = true;
            gameObject.SetActive(false);
            return;
        }
        
        isCollected = false;
        gameObject.SetActive(true);
    }

}
