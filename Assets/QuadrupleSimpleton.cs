using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using random = UnityEngine.Random;
using System.Text.RegularExpressions;

//note: all the fields/methods I'm not commenting should be auto-documentable
public class QuadrupleSimpleton : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule M;
    public KMSelectable Module;
    public KMSelectable RefButton;
    public GameObject StatusLight;
        public KMRuleSeedable RuleSeed;
        private int seed;

    private int side; //how many buttons will the NxN square have?
    private bool solved;
    private float originalY; //original y position of the button
    private GameObject workaround; //see WorkaroundTheWarning
    private ButtonBehaviour behaviour;
    private List<KMSelectable> buttons = new List<KMSelectable>();
        int moduleId;

    //note: I would've put "side" as a local variable, but TP exists so.

    private void Awake()
    {
        if (TwitchPlaysActive) ModuleLog("TP KTANE IS ACTIVE.");
        moduleId++; //every module has to have an ID number, starting from 1
        ModuleLog("T means Top, B means Bottom, L means Left, R means Right.");
        originalY = RefButton.transform.localPosition.y; //see ButtonPush
        seed = RuleSeed.GetRNG().Seed; //note: GetRNG just returns a MonoRandom which has the property I need ("Seed")
        MakeBehaviourDecision();
        WorkaroundTheWarning(); //see WorkaroundTheWarning
        MakeButtons(); //put the neccessary number of buttons on the module
    }

    //no explanation needed
    private void ModuleLog(string message) {
        Debug.LogFormat("[Quadruple Simpleton #{0}] {1}", moduleId, message);
    }

    /* It does two things: (based off "seed")
     * - Sets "side" a value
     * - Modifies the status light (to be visible or not)
     */
    private void MakeBehaviourDecision()
    {
        if (seed == 1)
            side = 2;
        else
        {
            side = RuleSeed.GetRNG().Next(9) % 9 + 3; //Interval: [3, 11]
            StatusLight.SetActive(false);
        }
    }

    //workaround for the warning: "You are trying to create a MonoBehaviour using the 'new' keyword. This is not allowed. MonoBehaviours can only be added using AddComponent()."
    private void WorkaroundTheWarning()
    {
        workaround = new GameObject();
        workaround.AddComponent<ButtonBehaviour>();
        ButtonBehaviour.PutConstructorPropierty(ref workaround, side);
        behaviour = workaround.GetComponent<ButtonBehaviour>();
    }

    private void MakeButtons()
    {
        for (int i = 0; i < side * side; i++)
        {
            Transform clone = Instantiate(RefButton.transform, RefButton.transform.parent);
            clone.transform.localScale =
                behaviour.CalculateSize(
                    clone.transform.localScale.x,
                    clone.transform.localScale.y,
                    clone.transform.localScale.z);
            clone.transform.localPosition =
                behaviour.CalculatePositions(
                    i, clone.transform.localPosition.y);

            buttons.Add(clone.GetComponent<KMSelectable>());
        }
        Module.Children = buttons.ToArray();
        Module.UpdateChildrenProperly();
        Destroy(RefButton.gameObject);
        HookButtons(buttons);
    }
    private void HookButtons(List<KMSelectable> buttons) {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].OnInteract += ButtonHandlerDelegateInstance(buttons[i], i);
    }

    private void PlayButtonAudio() { Audio.PlaySoundAtTransform("Victory", Module.transform); }
    private void DoEasterEgg()
    {
        Audio.PlaySoundAtTransform("Dora", Module.transform);
        buttons[0].GetComponentInChildren<TextMesh>().text = "¡Lo";
        buttons[1].GetComponentInChildren<TextMesh>().text = "hicimos!";
        buttons[2].GetComponentInChildren<TextMesh>().text = "We did";
        buttons[3].GetComponentInChildren<TextMesh>().text = "it!";
        ModuleLog("You did it!! Congrats! :D");
    }

    KMSelectable.OnInteractHandler ButtonHandlerDelegateInstance(KMSelectable b, int p) {
        return delegate { ButtonHandler(b, p); return false; };
    }

    private void ButtonHandler(KMSelectable button, int position)
    {
        bool pressed = button.GetComponentInChildren<TextMesh>().text != "PUSH IT!"; //just an abstraction

        if (pressed)
        {
            ModuleLog(behaviour.AgainMessage(position));
            Audio.PlaySoundAtTransform("Bounce", button.transform);
            //already been solved?
            if (solved) button.AddInteractionPunch(100f);
            else StartCoroutine(ButtonPush(button.transform));
        }
        else
        {
            PlayButtonAudio();
            button.AddInteractionPunch();
            ModuleLog(behaviour.ButtonMessage(position));
            button.GetComponentInChildren<TextMesh>().text = "VICTORY!";
            //not solved? check
            solved = behaviour.CheckSolve();
            if (solved)
            {
                M.HandlePass();
                ModuleLog("SOLVED!");
                if (seed == 1)
                { if (random.Range(0, 50) == 0) DoEasterEgg(); }
                else StartCoroutine(RandomSolved());
            }
        }
    }

    private void ColorButtons(Color color)
    {
        foreach (KMSelectable button in buttons)
            button.GetComponentInChildren<TextMesh>().color = color;
    }
    IEnumerator RandomSolved()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
        for (int i = 0; i < 2; i++)
        {
            ColorButtons(Color.green);
            yield return new WaitForSeconds(0.4f);
            ColorButtons(Color.white);
            yield return new WaitForSeconds(0.4f);
        }
        ColorButtons(new Color(0, 0.8f, 0));
    }

    IEnumerator ButtonPush(Transform pressedButtonTransform) //if you jitter click you can sink the button (...if you replace originalY for pressedButtonTransform.localPosition.y)
    {
        pressedButtonTransform.localPosition =
            new Vector3(pressedButtonTransform.localPosition.x,
                        originalY - 0.002f,
                        pressedButtonTransform.localPosition.z);
        yield return new WaitForSeconds(0.2f);
        pressedButtonTransform.localPosition =
            new Vector3(pressedButtonTransform.localPosition.x,
                        originalY,
                        pressedButtonTransform.localPosition.z);
    }

#pragma warning disable 414 //created but not used
    private readonly string TwitchHelpMessage =
        "Use <<!{0} (press|p|button|b) n>> to press the *n*th button (spaces are optional). The button order is in reverse reading order." +
        "Also, you can do <<!{0} nothing>> to do nothing (as if you were actually doing something to solve the module, huh)." +
        "You can chain commands (where n is) within the range [0, p], where *p* is `⌈(21/31)√b⌉`, and *b* is the number of buttons on the module.";
    //to be fair, I could've put "((p)ress or (b)utton)", but people can understand that as "only p or b".
    //Alternatively, I could put (p(ress) or b(utton)), which is the one that makes the most sense, but that would confuse people
#pragma warning restore 414
    bool TwitchPlaysActive;

    private string parseChainCommand(string input)
    {
        Regex commandRegex = new Regex(@"^((press|p|button|b)?\s?(?<number>[1-4])\s?)+$|nothing$"); //I like highlights :)
        Match commands =
            Regex.Match(input, string.Format(@"^((press|p|button|b)?\s?(?<number>[1-{0}])\s?)+$|nothing$", side * side),
            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace); //flags: 100101
        if (commands.Success)
        {
            int chainLimit = Mathf.CeilToInt(21f / 31 * side);
            int presses = commands.Groups["number"].Captures.Count;
            if (presses > chainLimit) return string.Format("sendtochat Sorry! You exceeded the number of buttons you can press at a time, which in this case is {0} (you tried to press {1}). I would've striked you, but I feel lazy.\tThe end? Question mark???", chainLimit, presses);
            else return commands.Groups["number"].Captures.Cast<Capture>().ToArray().Join();
        }
        return "sx";
    }

#pragma warning disable 414 //created but not used
    IEnumerator ProcessTwitchCommand(string input)
#pragma warning restore 414 //created but not used
    {
        string logMessage = string.Format("You did the command: <<{0}>>. ", input);
        string chainReturnValue = parseChainCommand(input).Trim();
        if (chainReturnValue[0] != 's')
        {
            logMessage += "(valid)";
            ModuleLog(logMessage);
            if (chainReturnValue.ToLowerInvariant() == "nothing")
            {
                ModuleLog("You did the \"nothing\" command. u funni person eh");
                yield return "sendtochat YES! YOU DID NOTHING! WOOHOO!! {0}{0}{0}{0}";
            }
            else
                foreach (int number in Array.ConvertAll(chainReturnValue.Split(), Convert.ToInt32))
                    buttons[number - 1].OnInteract();

            yield return null;
        }
        else
        {
            logMessage += "(wrong: ";
            if (chainReturnValue[1] == 'e')
            {
                yield return chainReturnValue;
                logMessage += "too many buttons chained, ";
            }
            logMessage += "not executed)";
            ModuleLog(logMessage);
        }

        yield break;
    }
}