using System.Collections;
using System.Linq;
using UnityEngine;

public class annoyingScript : MonoBehaviour {

    public KMSelectable[] buttons;
    public TextMesh[] texts;
    public MeshRenderer[] buttonMesh;
    public KMBombModule Module;
    public Material[] materials = new Material[4];
    public MeshRenderer moduleMesh;
    public AudioClip[] sounds = new AudioClip[3];
    public KMAudio Audio;

    private int[] numbers;
    private bool enc;
    private bool SOLVED;
    private int pressed;

    static int ModuleIdCounter;
    int ModuleId;
    
    int getMin()
    {
        return numbers.IndexOf(x => x == numbers.Min());
    }

    void process()
    {
        Audio.PlaySoundAtTransform(sounds[1].name, transform);
        if (!enc)
        {
            enc = true;
            randomize();
        }
        else
        {
            SOLVED = true;
            Module.HandlePass();
            moduleMesh.material = materials[2];
            for (int i = 0; i < 16; i++)
            {
                texts[i].text = "☺☻☺";
                buttonMesh[i].material = materials[3];
            }

        }
    }

    void Press(int s)
    {
        if (SOLVED || numbers[s] == 999) return;
        Debug.Log("[Annoying Puzzle #"+ModuleId+"] " + numbers[s] + " pressed. " + (s == getMin()?"Correct.":"Strike."));
        if (s == getMin())
        {
            texts[s].text = "";
            pressed++;
            numbers[s] = 999;
            if (pressed == 16) process();
            else
            {
                Audio.PlaySoundAtTransform(sounds[0].name, transform);
                Debug.Log("[Annoying Puzzle #"+ModuleId+"] " +"Next up: " + numbers.Min());
            }
        }
        else
        {
            randomize();
            Audio.PlaySoundAtTransform(sounds[2].name, transform);
            Module.HandleStrike();
        }
    }

    string toStr(int num)
    {
        return Enumerable.Range(0, num.ToString().Length).Select(x => ")!@#$%^&*("[num.ToString()[x] - '0'].ToString()).Aggregate((a, b) => a + b);
    }

    void randomize()
    {
        pressed = 0;
        numbers = Enumerable.Range(0, 250).ToList().Shuffle().Take(16).ToArray();
        for (int i = 0; i < 16; i++)
            texts[i].text = enc ? toStr(numbers[i]) : numbers[i].ToString();
        Debug.Log("[Annoying Puzzle #"+ModuleId+"] " +"Next up: " + numbers.Min());
    }

	void Start () {

        ModuleId = ModuleIdCounter++;
        ModuleId++;
        for (int i = 0; i < 16; i++)
        {
            int i1 = i;
            buttons[i1].OnInteract += delegate { Press(i1); return false; };
        }
        randomize();
        moduleMesh.material = materials[0];
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage =  "Use !{0} press # to press button on this position. Buttons are numbered 0-15 in reading order.";
    private bool TwitchPlaysActive = false;
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        Command = Command.ToLower();
        if (!Command.RegexMatch("press ([0-9])+")) yield return "sendtochaterror Invalid command!";
        else {
            int? num = Command.Substring(6).TryParseInt();
            if (num == null || num<0 || num>15) yield return "sendtochaterror Invalid command!";
            else Press((int)num);
        }
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!SOLVED)
        {
            Press(getMin());
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
