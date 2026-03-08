using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    public GameObject cutsceneUI;
    public GameObject player;
    public GameObject playerUI;

    public GameObject stoneman;

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
        player.GetComponent<PlayerInteract>().enabled = false;
        player.GetComponent<FlashlightController>().enabled = false;
        player.GetComponent<InputHandler>().enabled = false;

        stoneman.GetComponent<StoneManAI>().enabled = false;
        stoneman.GetComponent<StoneManMover>().enabled = false;
        stoneman.GetComponent<StoneManPatrol>().enabled = false;

        playerUI.SetActive(false);

        cutsceneUI.SetActive(true);

        DialogueManager.Instance.StartDialogue(index);
    }

    public void EndCutscene()
    {
        player.GetComponent<PlayerMovement2D>().enabled = true;
        player.GetComponent<PlayerInteract>().enabled = true;
        player.GetComponent<FlashlightController>().enabled = true;
        player.GetComponent<InputHandler>().enabled = true;

        stoneman.GetComponent<StoneManAI>().enabled = true;
        stoneman.GetComponent<StoneManMover>().enabled = true;
        stoneman.GetComponent<StoneManPatrol>().enabled = true;
        playerUI.SetActive(true);

        cutsceneUI.SetActive(false);
    }
}