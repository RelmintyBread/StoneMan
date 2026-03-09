using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance { get; private set; }

    [Header("Guide UI Panels")]
    [SerializeField] private GameObject movementGuideUI;
    [SerializeField] private GameObject flashlightGuideUI;
    [SerializeField] private GameObject interactGuideUI;
    [SerializeField] private GameObject saveGuideUI;

    private const string KeyGuideCompleted = "guide.completed";
    private const string KeyMovementDone = "guide.movement.done";
    private const string KeyFlashlightDone = "guide.flashlight.done";
    private const string KeyInteractDone = "guide.interact.done";
    private const string KeySaveDone = "guide.save.done";

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
        LoadGuideState();
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
        SaveBool(KeyMovementDone, true);
        RefreshGuideUI();
    }

    public void NotifyFlashlightUsed()
    {
        if (isGuideCompleted || !isMovementDone || isFlashlightDone) return;

        isFlashlightDone = true;
        SaveBool(KeyFlashlightDone, true);
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
        SaveBool(KeyInteractDone, true);
        RefreshGuideUI();
    }

    public void NotifySaveUsed()
    {
        if (isGuideCompleted || !isInteractDone || isSaveDone) return;

        isSaveDone = true;
        SaveBool(KeySaveDone, true);
        RefreshGuideUI();
    }

    [ContextMenu("Reset Guide Progress")]
    public void ResetGuideProgress()
    {
        PlayerPrefs.DeleteKey(KeyGuideCompleted);
        PlayerPrefs.DeleteKey(KeyMovementDone);
        PlayerPrefs.DeleteKey(KeyFlashlightDone);
        PlayerPrefs.DeleteKey(KeyInteractDone);
        PlayerPrefs.DeleteKey(KeySaveDone);
        PlayerPrefs.Save();

        LoadGuideState();
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
        SaveBool(KeyGuideCompleted, true);
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

    private void LoadGuideState()
    {
        isGuideCompleted = LoadBool(KeyGuideCompleted);
        isMovementDone = LoadBool(KeyMovementDone);
        isFlashlightDone = LoadBool(KeyFlashlightDone);
        isInteractDone = LoadBool(KeyInteractDone);
        isSaveDone = LoadBool(KeySaveDone);

        if (isGuideCompleted)
        {
            isMovementDone = true;
            isFlashlightDone = true;
            isInteractDone = true;
            isSaveDone = true;
        }
    }

    private static bool LoadBool(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
