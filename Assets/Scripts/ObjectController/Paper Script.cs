using UnityEngine;

public class PaperScript : MonoBehaviour, IInteractable, ISaveable
{
    [SerializeField] private string paperID;
    [SerializeField] private float requiredHoldTime = 3f;

    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool isCollected = false;

    [Header("Paper UI")]
    [SerializeField] private GameObject paperUI;

    public static class GameState
    {
        public static bool IsPaused = false;
    }

    void Awake()
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

        if (isCollected) return;

        isCollected = true;

        if (paperUI != null)
        {
            paperUI.SetActive(true);
            Time.timeScale = 0f; // Pause game
            GameState.IsPaused = true;
        }

        Debug.Log("Paper collected: " + paperID);

        gameObject.SetActive(false);
    }

    // 🔹 Button will call this
    public void ClosePaper()
    {
        if (paperUI != null)
        {
            paperUI.SetActive(false);
            Time.timeScale = 1f; // Resume game
            GameState.IsPaused = false;
        }
    }

    // ===== Save System =====

    public void OnSave(SaveData data)
    {
        if (isCollected && !data.isPaperCollected.Contains(paperID))
        {
            data.isPaperCollected.Add(paperID);
        }
    }

    public void OnLoad(SaveData data)
    {
        if (data.isPaperCollected.Contains(paperID))
        {
            isCollected = true;
            gameObject.SetActive(false);
            return;
        }

        isCollected = false;
        gameObject.SetActive(true);
    }
}