using UnityEngine;

public class Artifact : MonoBehaviour, IInteractable
{
    [SerializeField] private string artifactID;   // UNIQUE ID per artifact
    [SerializeField] private float requiredHoldTime = 3f;

    private float currentHoldTime = 0f;
    private bool isHolding = false;

    public static int collectedArtifacts = 0;
    public static int totalArtifactsRequired = 5;

    private void Awake()
    {
        // If already collected before, destroy immediately
        if (PlayerPrefs.GetInt("Artifact_" + artifactID, 0) == 1)
        {
            Destroy(gameObject);
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

    public void ShowInteractUI()
    {
        UIHandler.Instance?.ShowInteractPrompt();
    }

    public void HideInteractUI()
    {
        UIHandler.Instance?.HideInteractPrompt();
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

        collectedArtifacts++;

        // SAVE that this specific artifact was collected
        PlayerPrefs.SetInt("Artifact_" + artifactID, 1);

        // Save updated count
        PlayerPrefs.SetInt("CollectedArtifacts", collectedArtifacts);

        PlayerPrefs.Save();

        Debug.Log("Artifact collected! Total: " + collectedArtifacts + "/" + totalArtifactsRequired);

        Destroy(gameObject);
    }

    void HandleUpdateUI()
    {
        if (UIHandler.Instance == null) return;
        UIHandler.Instance.SetArtifactUI(collectedArtifacts, totalArtifactsRequired);
    }
}