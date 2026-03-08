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

    // Static variables to track collected artifacts across all instances
    private PlayerInteract playerInteract;

    private void Awake()
    {
        playerInteract = PlayerInteract.Instance;
        SaveManager.RegisterSaveable(this);
    }

    private void Start()
    {
        if (playerInteract == null)
        {
            playerInteract = PlayerInteract.Instance;
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

        HandleUpdateUI();
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
    }

    public void StopInteract()
    {
        isHolding = false;
        currentHoldTime = 0f;
    }

    public void Interact()
    {
        isHolding = false;

        if (isCollected)
        {
            return;
        }

        if (playerInteract == null)
        {
            playerInteract = PlayerInteract.Instance;
            if (playerInteract == null)
            {
                return;
            }
        }

        playerInteract.collectedArtifacts++;
        isCollected = true;

        Debug.Log("Artifact collected! Total: " + playerInteract.collectedArtifacts + "/" + playerInteract.totalArtifactsRequired);

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
        }
    }

    // ===== UI Update Handler =====
    void HandleUpdateUI()
    {
        if (UIGameHandler.Instance == null || playerInteract == null) return;
        UIGameHandler.Instance.SetArtifactUI(playerInteract.collectedArtifacts, playerInteract.totalArtifactsRequired);
    }
}
