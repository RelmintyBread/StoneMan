using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
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

    [Header("Image UI 2")]
    public Image fadeImage2;
    public CanvasGroup fadeCanvas2;
    public float fadeDuration2 = 2f;
    public float typingSpeed2 = 0.05f;

    RectTransform textRect;

    int index = 0;
    bool isTyping = false;
    bool skipTyping = false;

    string[] lines =
    {
        "<color=white>14th August 1972 \nIn the peaceful night, somewhere in the unknown...A bunch of scums tried to sell a very special drugs that could light in the dark. They called it “Miracle”. However, their transaction ended up in a bloody warfare. Five men rallying with their car while hasten up by their furious client. Unfortunately, a cliff swallow them. Four of the scums are dead, leaving only one survivor, George Cranton....,</color>",
        "“oohh...no....no....no, no, no....”",
        "“J’s?...Javier? Wake up mate, plese....(sight) wake up....”",
        "“We should get out of here...!”",
        "“.....”",
        "“Gods....”",
        "“I am sorry, J’s....I am really sorry.... (sigh)”",
        "But, I should go before they could catch me!",
        "Wait, what is this place? A mansion? In the woods?",
        "<color=orange>Where is that g*ddamn son of a b*tch!? Bring us his head! Boss would like a fairest trophy!</color>",
        "Shit, I guess i have no other choice, this place could be a safe spot for hiding",
        "(Opening Door)",
        "It’s stinks, looks like this place has been abandoned for a hundred years...",
        "And why is it so dark in-",
        "!!!",
        "What...? What in the blo*dy hell is that?",
        "Arrghh...f*ck!!! (burn hot when touching the magic seal)",
        "D*mnit, I can’t get out, this place is cursed!",
        "Oh gods, I should listen to Emily from the start....huhh...this is so f*cked up...",
        "(sigh)....",
        "Huuh...there must be something here to lift the curse. I must check it out...",
        "",
        ""
    };

    void Start()
    {
        Time.timeScale = 0f;
        blackScreen.SetActive(true);

        textRect = text.GetComponent<RectTransform>();

        StartCoroutine(TypeLine());
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
        if (index == 0)
        {
            textRect.anchoredPosition = new Vector2(-77, 440);
        }
        else if (index <= 6)
        {
            textRect.anchoredPosition = new Vector2(-100, 0);
        }
        else
        {
            textRect.anchoredPosition = new Vector2(-106, -339);
        }
    }

    void UpdateTextAlignment()
    {
        if (index == 0)
            text.alignment = TextAlignmentOptions.TopLeft;
        else
            text.alignment = TextAlignmentOptions.Center;
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        skipTyping = false;

        UpdateTextAlignment();
        UpdateTextPosition();

        string currentLine = lines[index];

        UpdateBorderColor(currentLine);

        text.text = "";

        for (int i = 0; i < currentLine.Length; i++)
        {
            if (skipTyping)
            {
                text.text = currentLine;
                break;
            }

            if (currentLine[i] == '<')
            {
                while (i < currentLine.Length && currentLine[i] != '>')
                {
                    text.text += currentLine[i];
                    i++;
                }

                if (i < currentLine.Length)
                    text.text += '>';
            }
            else
            {
                text.text += currentLine[i];
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
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

    IEnumerator FadeInImage2()
    {
        yield return StartCoroutine(Fade(fadeCanvas2, fadeDuration2, 0f, 1f));
    }

    IEnumerator FadeOutImage2()
    {
        yield return StartCoroutine(Fade(fadeCanvas2, fadeDuration2, 1f, 0f));
    }

void NextLine()
{
    index++;

    switch (index)
    {
        case 7:
            dialogueBox.SetActive(true);
            break;

        case 19:
            StartCoroutine(FadeInImage());
            break;

        case 20:
            StartCoroutine(FadeOutImage());
            break;

        case 21:
            dialogueBox.SetActive(false);
            StartCoroutine(FadeInImage2());
            break;

        case 22:
            StartCoroutine(FadeOutImage2());
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
        gameObject.SetActive(false);
    }

    void UpdateBorderColor(string line)
    {
        if (dialogueBorder == null) return;

        Color targetColor = Color.blue;

        if (line.Contains("<color=orange>"))
            targetColor = Color.orange;
        else if (line.Contains("<color=white>"))
            targetColor = Color.white;

        dialogueBorder.color = targetColor;
    }
}