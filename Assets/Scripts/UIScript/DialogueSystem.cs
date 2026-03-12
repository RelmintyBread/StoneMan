using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header ("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image dialogueBorder;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    public string[][] dialogues;

    private int currentLine;
    private string[] currentDialogue;

    bool isTyping = false;
    bool skipTyping = false;

    private void Awake()
    {
        Instance = this;

        dialogues = new string[][]
        {
            new string[]
            {
                "♫ Hark now, ye travelers of the winding way",
                "♫ And hear a tale of night turned into day.",
                "♫ Until there rose, with eagle’s eye and hand of steel,",
                "♫ Marcus Aurelius, to whom the fates did kneel.",
                "<color=red>.....</color>",
                "<color=red>Boy, such a splendid voice you have, the Four Crest isn’t it?</color>",
                "o-ouh...ummm...L-lady Fornsword?! Y-yem mit is, M’Lady...",
                "<color=red>Afraid of interactions? Hahaha...What is your name, boy?</color>",
                "Y-yes M’Lady...it’s Eobard, people used to call be “Bard”",
                "<color=red>“Bard”? Wouldn’t that be a coincidence?</color>",
                "<color=red>Do you know to sing “Slovenikir’s Wrath”</color>",
                "Yes I-I do, M’Lady...?",
                "<color=red>Then come with me, boy. I would like an addision in mansion. Charles would be happy to hear your voice, or course</color>",
                "W-what...? M’Lady,I just a carpenter, Mr. Wuderbraum wouln’t let me-",
                "<color=red>(hand sight to shut down) Come on, up here. I’ve bough you from your master</color>",
                "<color=red>From now on, you are working for me, understand?</color>",
                "Y-yes, M’Lady! Bard will be your most loyal servant!",
                "<color=red>Good.</color>"
            },

            new string[]
            {
                "(walk northen corridor)",
                "<color=red>(crying from library)</color>",
                "(peeking from the door)",
                "<color=red>O Charles....why?</color>",
                "<color=red>Why did you go to that battle?</color>",
                "<color=red>Such a pride aracanist you are...a gentleman you are...</color>",
                "<color=red>We used to feel each other’s warm...but now I can’t feel you anymore!</color>",
                "<color=red>Oh dear gods, why?(crying)</color>",
                "(feels the intense pain, willing to give a support)",
                "(hesitant kicks in, decide to go away)"
            },

            new string[]
            {
                "<color=red>Bard, how do I look?</color>",
                "M’Lady, if Bard permitted to say...",
                "Across all of Harithal, even the great queen of Carista herself are no match of your fabulous!",
                "<color=red>That’s a bit...hyperbolic...Carista are one of Westcrest trade partner, don’t forget that</color>",
                "uuhh...(gulp)",
                "<color=red>But I like the compliment to gave! Especially this brooch...this was the last gift from Charles for me</color>",
                "<color=red>Before his souless body come home</color>",
                "(sight, looking down sad)....",
                "<color=red>But, that’s years ago, he’s such a- (cought)</color>",
                "Umm my apologize M’Lady, would you like Bard to call the doctor?",
                "<color=red>Urhmm...yes, I am afraid this symtomes are getting worst.</color>",
                "<color=red>But, we’ll getting guest at noon. A Gundharian, you know what to do, aren’t you?</color>",
                "Of course M’Lady! Such a splendid performace shall be given by the wonderful Bard tonight!",
                "<color=red>Hahahaha (cought)...that’s the spirit (smile)</color>"
            },

            new string[]
            {
                "(holding tears) M’Lady, please...get rest...It has become worse since the last time",
                "Perhaps, other time, Bard will wander to Vivac. There must be the cure...",
                "<color=red>(lying in bed, coughing) Bard...</color>",
                "<color=red>I...have no other time</color>",
                "<color=red>I...I can feel the warm again, it’s Charles</color>",
                "<color=red>My wonderful husband are holding me right now...</color>",
                "(still trying holding tear, but a few drops)",
                "Oh dear M’Lady, would you like Bard to fetch you a song?",
                "<color=red>That....(cough)....will be nice</color>",
                "(strengthen self)",
                "(strengthen self)",
                "♫ So drink to the Lady, so noble and bright,",
                "♫ Who keepeth the darkness away from our sight.",
                "♫ May her name be whispered while the stars still shine,",
                "<color=red>(smiling, breath stopping, the body are souless...now)</color>",
                "♫ The Crimson Lady—the soul of the line.",
                "(crying, can’t holding tear anymore)",
                "why.....",
                "(scream) WHYYYYY....????!!!!",
                "(sigh)...Dear gods, oh my mighty gods...Why you take M’Lady away from me? Why....(nod)",
                "why....."
            },

            new string[]
            {
                "This is it, this shall works!",
                "Seperate the soul information into 5 different item then recombining them again",
                "Such an easy thing to do, why does Sir Charles forbid it?",
                "Nothing, this shall wake The Crimson Lady, again!",
                "!!!",
                "W-what is happening??",
                "(the skin start to get hard as rock)",
                "URRRGGHHHH!!! (body growing unctrolled)",
                "AARRGHHHH, WHAT HAPPEN? I DID EVERYTHING FOR THIS!",
                "UUUGGGHHH....I am starting to losing my mind....",
                "Ohhh gods, please....please hear my plea....please forgive my stubbornness....",
                "(start to forget who he is) MY NAME IS BARD, CRIMSON LADY HUMBLE SERVANT!",
                "MY NAME IS BARD, CRIMSON LADY HUMBLE SERVANT!",
                "M-my name is Bard, Crimson Lady....",
                "M-....M-my....name....",
                "is.....",
                "Bard....",
                "<color=gray>(disturb voice) bard....</color>",
                "<color=gray>Crimson....Lady.....</color>",
                "<color=gray>Bard....</color>"
            }

        };
    }

    public void StartDialogue(int index)
    {
        currentDialogue = dialogues[index - 1];
        currentLine = 0;

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
        CutsceneManager.Instance.EndCutscene();
    }
}
IEnumerator TypeLine()
{
    isTyping = true;
    skipTyping = false;
    dialogueText.text = "";

    string line = currentDialogue[currentLine];

    UpdateBorderColor(line);

    for (int i = 0; i < line.Length; i++)
{
    if (skipTyping)
    {
        dialogueText.text = line;
        break;
    }

    if (line[i] == '<') // Detect rich text tag
    {
        while (i < line.Length && line[i] != '>')
        {
            dialogueText.text += line[i];
            i++;
        }

        dialogueText.text += '>';
    }
    else
    {
        dialogueText.text += line[i];
        yield return new WaitForSeconds(typingSpeed);
    }
}

    isTyping = false;
}

    void NextLine()
    {
        currentLine++;

        if (currentLine >= currentDialogue.Length)
        {
            CutsceneManager.Instance.EndCutscene();
            return;
        }

        StartCoroutine(TypeLine());
    }

void UpdateBorderColor(string line)
{
    if (dialogueBorder == null) return;

    Color targetColor = Color.orange;

    if (line.Contains("<color=red>"))
    {
        targetColor = Color.red;
    }
    else if (line.Contains("<color=gray>"))
    {
        targetColor = Color.gray;
    }

    if (dialogueBorder != null)
        dialogueBorder.color = targetColor;

}
}