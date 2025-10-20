using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;

public class annoyingScript : MonoBehaviour {

    public KMSelectable[] buttons = new KMSelectable[25];
    public TextMesh[] texts = new TextMesh[25];
    public MeshRenderer[] buttonMesh = new MeshRenderer[25];
    public KMBombModule Module;
    public Material[] materials = new Material[4];
    public MeshRenderer moduleMesh;
    public AudioClip[] sounds = new AudioClip[3];
    public KMAudio Audio;

    private int[] numbers = new int[25];
    private bool enc = false;
    private bool SOLVED = false;
    private const string sym = ")!@#$%^&*(";
    private int pressed = 0;

    static int ModuleIdCounter = 0;
    int ModuleId;


    int getMin()
    {
        int min = numbers[0];
        int ind = 0;
        for (int i=1; i<25; i++)
        {
            if (numbers[i] < min)
            {
                min = numbers[i];
                ind = i;
            }
        }
        return ind;
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
            for (int i = 0; i < 25; i++)
            {
                texts[i].text = "☺☻☺";
                buttonMesh[i].material = materials[3];
            }

        }
    }

    bool Press(int s)
    {
        if (!SOLVED && numbers[s]!=999) {
            Debug.Log(numbers[s] + " pressed. " + ((s == getMin())?"Correct.":"Strike."));
            if (s == getMin())
            {
                texts[s].text = "";
                pressed++;
                numbers[s] = 999;
                if (pressed == 25) process();
                else
                {
                    Audio.PlaySoundAtTransform(sounds[0].name, transform);
                    Debug.Log("Next up: " + numbers[getMin()]);
                }
            }
            else
            {
                randomize();
                Audio.PlaySoundAtTransform(sounds[2].name, transform);
                Module.HandleStrike();
            }
        }
        return true;
    }

    string toStr(int num)
    {
        string ans = "";
        if (num / 100 != 0) ans += sym[num / 100];
        if ((num / 100 != 0) || (num / 10 % 10!=0)) ans += sym[num / 10 % 10];
        ans += sym[num % 10];
        return ans;
    }

    void randomize()
    {
        pressed = 0;
        bool rep;
        for (int i = 0; i < 25; i++)
        {
            buttonMesh[i].material = materials[1];
            rep = true;
            while (rep)
            {
                rep = false;
                numbers[i] = Random.Range(0, 250);
                for (int j=0; j<i; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        rep = true;
                        break;
                    }
                }
            }
            texts[i].text = enc ? toStr(numbers[i]) : numbers[i].ToString();
        }
        Debug.Log("Next up: " + numbers[getMin()]);
    }

	void Start () {

        ModuleId = ModuleIdCounter++;
        ModuleId++;

        buttons[0].OnInteract += delegate () { return Press(0); };
        buttons[1].OnInteract += delegate () { return Press(1); };
        buttons[2].OnInteract += delegate () { return Press(2); };
        buttons[3].OnInteract += delegate () { return Press(3); };
        buttons[4].OnInteract += delegate () { return Press(4); };
        buttons[5].OnInteract += delegate () { return Press(5); };
        buttons[6].OnInteract += delegate () { return Press(6); };
        buttons[7].OnInteract += delegate () { return Press(7); };
        buttons[8].OnInteract += delegate () { return Press(8); };
        buttons[9].OnInteract += delegate () { return Press(9); };
        buttons[10].OnInteract += delegate () { return Press(10); };
        buttons[11].OnInteract += delegate () { return Press(11); };
        buttons[12].OnInteract += delegate () { return Press(12); };
        buttons[13].OnInteract += delegate () { return Press(13); };
        buttons[14].OnInteract += delegate () { return Press(14); };
        buttons[15].OnInteract += delegate () { return Press(15); };
        buttons[16].OnInteract += delegate () { return Press(16); };
        buttons[17].OnInteract += delegate () { return Press(17); };
        buttons[18].OnInteract += delegate () { return Press(18); };
        buttons[19].OnInteract += delegate () { return Press(19); };
        buttons[20].OnInteract += delegate () { return Press(20); };
        buttons[21].OnInteract += delegate () { return Press(21); };
        buttons[22].OnInteract += delegate () { return Press(22); };
        buttons[23].OnInteract += delegate () { return Press(23); };
        buttons[24].OnInteract += delegate () { return Press(24); };
        randomize();
        moduleMesh.material = materials[0];
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage =
        @"Use !{0} press # to press button on this position. Buttons are numbered 0-24 in reading order.";
    private bool TwitchPlaysActive = false;
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        Command = Command.ToLower();
        if (!Command.RegexMatch("press ([0-9])+")) yield return "sendtochaterror Invalid command!";
        else {
            int? num = Command.Substring(6).TryParseInt();
            if (num == null || num<0 || num>24) yield return "sendtochaterror Invalid command!";
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
