using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    public GameObject cutsceneUI;
    public GameObject player;
    public GameObject playerUI;

    private int lastPlayedCutscene = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void TryPlayCutscene(int artifactCount)
    {
        if (artifactCount <= lastPlayedCutscene) return;

        lastPlayedCutscene = artifactCount;

        StartCutscene(artifactCount);
    }

    void StartCutscene(int index)
    {
        player.GetComponent<PlayerMovement2D>().enabled = false;
        playerUI.SetActive(false);

        cutsceneUI.SetActive(true);

        DialogueManager.Instance.StartDialogue(index);
    }

    public void EndCutscene()
    {
        player.GetComponent<PlayerMovement2D>().enabled = true;
        playerUI.SetActive(true);

        cutsceneUI.SetActive(false);
    }
}