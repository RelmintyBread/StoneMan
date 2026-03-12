using UnityEngine;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    public GameObject cutsceneUI;
    public GameObject player;
    public GameObject playerUI;
    public GameObject stoneman;

    [Header("Fade UI")]
    public GameObject fadeUI;
    private CanvasGroup fadeGroup;
    public float fadeSpeed = 2f;

    private int lastPlayedCutscene = 0;

    private void Awake()
    {
        Instance = this;
        fadeGroup = fadeUI.GetComponent<CanvasGroup>();
    }

    public void TryPlayCutscene(int artifactCount)
    {
        if (artifactCount <= lastPlayedCutscene) return;

        lastPlayedCutscene = artifactCount;

        StartCoroutine(FadeAndStart(artifactCount));
    }

    IEnumerator FadeAndStart(int index)
    {
        yield return StartCoroutine(FadeIn());

        StartCutscene(index);
    }

    IEnumerator FadeIn()
    {
        fadeUI.SetActive(true);

        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        while (fadeGroup.alpha > 0)
        {
            fadeGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        fadeUI.SetActive(false);
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
        StartCoroutine(FadeEnd());
    }

    IEnumerator FadeEnd()
    {
        yield return StartCoroutine(FadeOut());

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