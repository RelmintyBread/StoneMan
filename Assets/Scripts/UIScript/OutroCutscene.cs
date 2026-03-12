using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class OutroCutscene : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject blackScreen;
    public TextMeshProUGUI text;
    public Image dialogueBorder;
    public GameObject dialogueBox;

    [Header("Image UI 1")]
    public Image fadeImage;
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 2f;
    public float typingSpeed = 0.05f;

    [Header("Ending Screen")]
    public GameObject thanksScreen;

    RectTransform textRect;

    int index = 0;
    bool isTyping = false;
    bool skipTyping = false;

    string[] lines =
    {
       ".....",
        "All set, I hope these things work",
        "I am not an arcanist, but let’s hope this is worth it",
        "<color=#808080>(heavy step lurking from behind)</color>",
        "SH*T!! STAY BACK!!",
        "(battery runs out)",
        "Ohh...F*ck....",
        "<color=#808080>(heavy step walking, hand rises, striking prepared)</color>",
        "<color=red>.....</color>",
        "<color=red>Charles...no, Bard....is that you?</color>",
        "<color=#808080>(heavy step suddenly stop)</color>",
        "<color=#808080>Voice...those....Crimson Lady....</color>",
        "<color=#808080>Knew Bard....success Bard....Alive is, Crimson Lady...!!</color>",
        "<color=red>Ohh Bard, my humble Bard...thee are such a loyal servant</color>",
        "<color=red>Such a resplendent piety theu offer</color>",
        "<color=red>Shalt thou rewarded, hasten thou step closer, my Bard....</color>",
        "<color=#808080>Wish you...as....Crimson Lady....</color>",
        "<color=red>Yess...(voice become deeper, distorted, absurd)...YESS!</color>",
        "<color=red>Thou’l become whole, eternity till the end!</color>",
        //Cutscene
        "<color=red>Thou shalt serve me...TO THE VOID!</color>",
        "WHAT THE F*CK!!?",
        "<color=#808080>UURGGHH.....(grunting)</color>",
        "<color=red>THOU, MERE HUMAN...SPEAD MY WORD AND PARDON SHALL BE THINE!</color>",
        "<color=red>THE VOID ARE MARCHING, ENDLESS ARMY ARE PREPARED THROUGH THE FIRST OF ITS CRACK</color>",
        "<color=red>WE WILL COME BACK, LORD HAMUN SHALT COME BACK!</color>",
        "<color=red>YOU MERE MORTAL ARE NO COMPARE TO US!</color>",
        "<color=red>THE WHITE HAND ARE A PROBLEM NO MORE!</color>",
        "<color=red>FRAIGH ZREUS, HUMAN!</color>",
        "<color=red>(portal erupts, swallowing the stoneman to the void)</color>",
        //blackscreen
        "Sh*t...Lord Hamun are coming? I though he is just a tale...",
        "This thing must be important, but....",
        "Those people are still outside, I should get moving first!",
        "Gods, help me....",
        "<color=white>Lord Hamun will return...</color>",
        "<color=white>And so The White Hand....</color>",
        "<color=white>The End</color>",
    };

    void Start()
    {
        Time.timeScale = 0f;
        blackScreen.SetActive(true);

        textRect = text.GetComponent<RectTransform>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                skipTyping = true;
            else
                NextLine();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EndCutscene();
        }
    }


    void UpdateTextPosition()
    {
        if (index >= 33)
        {
            textRect.anchoredPosition = new Vector2(-678, 386);
        }
        if (index == 35)
        {
            textRect.anchoredPosition = new Vector2(-41, 52);
        }
    }

IEnumerator TypeLine()
{
    isTyping = true;
    skipTyping = false;

    UpdateTextPosition();

    string currentLine = lines[index];

    UpdateBorderColor(currentLine);

    text.text = "";

    int i = 0;

    while (i < currentLine.Length)
    {
        if (skipTyping)
        {
            text.text = currentLine;
            break;
        }

        if (currentLine[i] == '<') // start of rich text tag
        {
            int tagEnd = currentLine.IndexOf('>', i);

            if (tagEnd != -1)
            {
                string tag = currentLine.Substring(i, tagEnd - i + 1);
                text.text += tag;
                i = tagEnd + 1;
                continue;
            }
        }

        text.text += currentLine[i];
        i++;

        yield return new WaitForSecondsRealtime(typingSpeed);
    }

    isTyping = false;
}

    IEnumerator Fade(CanvasGroup canvas, float duration, float start, float end)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            canvas.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }

        canvas.alpha = end;
    }

    IEnumerator FadeInImage()
    {
        yield return StartCoroutine(Fade(fadeCanvas, fadeDuration, 0f, 1f));
    }

    IEnumerator FadeOutImage()
    {
        yield return StartCoroutine(Fade(fadeCanvas, fadeDuration, 1f, 0f));
    }

    void NextLine()
{
    index++;

    switch (index)
        {
            case 19: 
            StartCoroutine(FadeInImage());
            break;

            case 29:
            StartCoroutine(FadeOutImage());
            break;

            case 33:
            dialogueBox.SetActive(false);
            break;
        }

    if (index >= lines.Length)
    {
        EndCutscene();
    }
    else
    {
        StartCoroutine(TypeLine());
    }
}

void EndCutscene()
{
    blackScreen.SetActive(false);
    Time.timeScale = 1f;

    if (thanksScreen != null)
        thanksScreen.SetActive(true);

    gameObject.SetActive(false);
}

    void UpdateBorderColor(string line)
    {
        if (dialogueBorder == null) return;

        Color targetColor = Color.blue;

        if (line.Contains("<color=red>"))
            targetColor = Color.red;
        else if (line.Contains("<color=white>"))
            targetColor = Color.white;
        else if (line.Contains("<color=#808080>"))
            targetColor = Color.gray;

        dialogueBorder.color = targetColor;
    }
}