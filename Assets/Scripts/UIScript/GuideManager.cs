using UnityEngine;

public class GuideManager : MonoBehaviour, ISaveable
{
    public static GuideManager Instance { get; private set; }

    [Header("Guide UI Panels")]
    [SerializeField] private GameObject movementGuideUI;
    [SerializeField] private GameObject flashlightGuideUI;
    [SerializeField] private GameObject interactGuideUI;
    [SerializeField] private GameObject saveGuideUI;

    private bool isGuideCompleted;
    private bool isMovementDone;
    private bool isFlashlightDone;
    private bool isInteractDone;
    private bool isSaveDone;
    private bool isAnyInteractableDetected;
    private bool isSavePointDetected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SaveManager.RegisterSaveable(this);
    }

    private void Start()
    {
        RefreshGuideUI();
    }

    public void NotifyMovementInput(Vector2 moveInput)
    {
        if (isGuideCompleted || isMovementDone) return;
        if (moveInput.sqrMagnitude <= 0.001f) return;

        isMovementDone = true;
        RefreshGuideUI();
    }

    public void NotifyFlashlightUsed()
    {
        if (isGuideCompleted || !isMovementDone || isFlashlightDone) return;

        isFlashlightDone = true;
        RefreshGuideUI();
    }

    public void NotifyInteractableDetected(IInteractable interactable)
    {
        isAnyInteractableDetected = interactable != null;
        isSavePointDetected = interactable is SavePoint2D;

        if (isGuideCompleted) return;
        if (!isMovementDone || !isFlashlightDone) return;
        RefreshGuideUI();
    }

    public void NotifyInteractPressed(IInteractable interactable)
    {
        if (isGuideCompleted || interactable == null) return;
        if (!isMovementDone || !isFlashlightDone || isInteractDone) return;

        isInteractDone = true;
        RefreshGuideUI();
    }

    public void NotifySaveUsed()
    {
        if (isGuideCompleted || !isInteractDone || isSaveDone) return;

        isSaveDone = true;
        RefreshGuideUI();
    }

    [ContextMenu("Reset Guide Progress")]
    public void ResetGuideProgress()
    {
        // PlayerPrefs.DeleteKey(KeyGuideCompleted);
        // PlayerPrefs.DeleteKey(KeyMovementDone);
        // PlayerPrefs.DeleteKey(KeyFlashlightDone);
        // PlayerPrefs.DeleteKey(KeyInteractDone);
        // PlayerPrefs.DeleteKey(KeySaveDone);
        // PlayerPrefs.Save();

        RefreshGuideUI();
    }

    private void RefreshGuideUI()
    {
        if (CheckAndSetGuideCompleted())
        {
            HideAllGuideUI();
            return;
        }

        if (!isMovementDone)
        {
            ShowGuide(movementGuideUI);
            return;
        }

        if (!isFlashlightDone)
        {
            ShowGuide(flashlightGuideUI);
            return;
        }

        if (!isInteractDone)
        {
            if (isAnyInteractableDetected)
            {
                ShowGuide(interactGuideUI);
            }
            else
            {
                HideAllGuideUI();
            }
            return;
        }

        if (!isSaveDone)
        {
            if (isSavePointDetected)
            {
                ShowGuide(saveGuideUI);
            }
            else
            {
                HideAllGuideUI();
            }
            return;
        }

        HideAllGuideUI();
    }

    private bool CheckAndSetGuideCompleted()
    {
        if (isGuideCompleted)
        {
            return true;
        }

        bool shouldComplete = isMovementDone && isFlashlightDone && isInteractDone && isSaveDone;
        if (!shouldComplete)
        {
            return false;
        }

        isGuideCompleted = true;
        return true;
    }

    private void ShowGuide(GameObject target)
    {
        SetActiveIfExists(movementGuideUI, movementGuideUI == target);
        SetActiveIfExists(flashlightGuideUI, flashlightGuideUI == target);
        SetActiveIfExists(interactGuideUI, interactGuideUI == target);
        SetActiveIfExists(saveGuideUI, saveGuideUI == target);
    }

    private void HideAllGuideUI()
    {
        SetActiveIfExists(movementGuideUI, false);
        SetActiveIfExists(flashlightGuideUI, false);
        SetActiveIfExists(interactGuideUI, false);
        SetActiveIfExists(saveGuideUI, false);
    }

    private void SetActiveIfExists(GameObject obj, bool state)
    {
        if (obj != null)
        {
            obj.SetActive(state);
        }
    }

    // ─────────────────────────────────────────────
    //  SAVE/LOAD
    // ─────────────────────────────────────────────
    public void OnSave(SaveData data)
    {
        data.isGuideDone = isGuideCompleted;
        data.isMovementDone = isMovementDone;
        data.isFlashlightDone = isFlashlightDone;
        data.isInteractDone = isInteractDone;
        data.isSaveDone = isSaveDone;
    }

    public void OnLoad(SaveData data)
    {
        isGuideCompleted = data.isGuideDone;
        isMovementDone = data.isMovementDone;
        isFlashlightDone = data.isFlashlightDone;
        isInteractDone = data.isInteractDone;
        isSaveDone = data.isSaveDone;
        RefreshGuideUI();
    }
}
