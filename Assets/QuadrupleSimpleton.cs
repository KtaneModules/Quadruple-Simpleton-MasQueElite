using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using random = UnityEngine.Random;

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
    private bool muted = false;
    private GameObject workaround; //see WorkaroundTheWarning
    private ButtonBehaviour behaviour;
    private List<KMSelectable> buttons = new List<KMSelectable>();
        static int moduleIdCounter = 1;
        int moduleId;

    //note: I would've put "side" as a local variable, but TP exists so.

    private void Awake()
    {
        if (TwitchPlaysActive) ModuleLog("TP KTANE IS ACTIVE.");
        moduleId = moduleIdCounter++; //every module has to have an ID number, starting from 1. Although... The LFA needs both moduleIdCounter AND moduleId... Oh well
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
    private void DoEasterEgg(bool forced = false)
    {
        if (!muted) Audio.PlaySoundAtTransform("Lo-hicimos", Module.transform);

        string[] extraStrings;
        if (forced) extraStrings = new string[] { "do", "it!", "You", "didn't", "You forced solved. Shame." };
        else extraStrings = new string[] { "We did", "it!", "¡Lo", "hicimos!", "You did it!! Congrats! :D" };

        for (int i = 0; i < 4; i++)
            buttons[i].GetComponentInChildren<TextMesh>().text = extraStrings[i];

        ModuleLog(extraStrings[4]);
    }

    KMSelectable.OnInteractHandler ButtonHandlerDelegateInstance(KMSelectable b, int p) {
        return delegate { ButtonHandler(b, p); return false; };
    }

    private void ButtonHandler(KMSelectable button, int position)
    {
        bool pressed = button.GetComponentInChildren<TextMesh>().text != "PUSH IT!"; //just an abstraction

        if (pressed)
        {
            if (!muted) Audio.PlaySoundAtTransform("boing", button.transform);
            if (TwitchPlaysActive) button.AddInteractionPunch(100f);

            ModuleLog(behaviour.AgainMessage(position));
            //already been solved?
            if (solved) button.AddInteractionPunch(100f);
            else StartCoroutine(ButtonPush(button.transform));
        }
        else
        {
            if (!muted) PlayButtonAudio();
            if (TwitchPlaysActive) StartCoroutine(ButtonPush(button.transform));

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
        if (!muted) Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
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
        "Use <<!{0} (press|p|button|b) n>> to press the nth button (spaces are optional). Starting at 1, the order is in reverse reading order. " +
        "Also, you can do <<!{0} nothing>> to do nothing. " +
        "You can chain commands up to p buttons, where p is ⌈(21/31)√b⌉, and *b* is the number of buttons on the module. " +
        "You can mute the module as well; only if you are in ruleseed though. Just type <<mute>> or <<m>>, BUT that would make me sad :(";
    //to be fair, I could've put "((p)ress or (b)utton)", but people can understand that as "only p or b"
    //alternatively, I could put (p(ress) or b(utton)), which is the one that makes the most sense, but that would confuse people
#pragma warning restore 414
#pragma warning disable 649 //not assigned and will always have the default value
    bool TwitchPlaysActive;
#pragma warning disable 649

    //the idea is: for example, take this array [132, 1, 5] and the threshold of 10, so... 132 -> 13 (/< 10) | 2 (separate the 13 from the 2, and comparing 13 with 10) -> <1> (< 10) | 32 => [1, 321, 5] => 321 -> 32 | 1 -> <3> | 21 => [1, 3, 215] ...
    private string ParseCommandNumbers(string ns)
    {
        int totalButtons = side * side;
        string resultArray = string.Empty; //the result should be something like "12 34 56"
        string[] numberArray = ns.Split(); //and yes, here I'm doing "12 34 56" -> ["12", "34", "56"]
        int length = numberArray.Length; //...because I need to see how many numbers do I have at the start
        int i = 0;
        while(i < length) //"customisable" foreach
        {
            string number = numberArray[i]; //I start with the first, and so on
            while (Convert.ToInt32(number) > totalButtons) //as I explained, if (the current number does not surpass the threshold), or "while" the opposite of that
            {
                if (numberArray.Length - 1 == i) //it this is the last element, where would I put the separated numbers then? This is the solution for that
                {
                    string remainingNumber = number; //on a further note, I have to say this solution is convoluted to avoid using List<T>s as much as possible, without it being too illegible
                    string storage = ""; //also do note this is not a brilliant nor a clunky implementation/solution, this is just what all real, respectable programmers do: solve problems applying quality solutions
                    do
                    {
                        storage = remainingNumber.Substring(remainingNumber.Length - 1) + storage; //same as numberBuffer: pick the last number and store it (for later)
                        remainingNumber = remainingNumber.Substring(0, remainingNumber.Length - 1); //pick the remaining number (from the left) to check it
                        if (Convert.ToInt32(remainingNumber) <= totalButtons) //if it's still too large, don't do anything and keep pulling out digits
                        {
                            resultArray += remainingNumber + " "; //the number is OK: put it in the result
                            remainingNumber = storage; //the new number now is what I got from the storage
                            storage = ""; //don't forget (that not)
                        }
                    } while (Convert.ToInt32(remainingNumber) > totalButtons);
                    number = remainingNumber; //so I can append the number at the end, at the line before i++
                    break;
                }
                string numberBuffer = number.Substring(number.Length - 1); //separate the digit one at a time from the end; this is the imaginary place the digit is in while the other part is being checked (or just assigned here, doesn't matter)
                number = number.Substring(0, number.Length - 1); //this is the new number I will be checking for the threshold
                numberArray[i+1] = numberBuffer + numberArray[i+1]; //put the cut number onto the next array element (you can say I "shift" it to the front of that number; and yes that is a JS moment)
            }
            length = numberArray.Length;
            resultArray += number + " ";
            i++;
        }
        return resultArray.ToString();
    }

    //welp, this is what you get if you want to allow the users to chain commands (and actually not braking the TP system of your module)
    private string ParseChainCommand(string input)
    {
        if (new string[] {"nothing", "mute", "m"}.Contains(input)) return input;
        Match commands =
            Regex.Match(input, @"^((press|p|button|b)?\s?(?<number>\d{1,3})\s?)+$", //love those highlights
            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace); //flags: 100101
        if (commands.Success)
        {
            int chainLimit = Mathf.CeilToInt(21f / 31 * side);
            string presses = ParseCommandNumbers(commands.Groups["number"].Captures.Cast<Capture>().ToArray().Join()).TrimEnd(); //there's an extra space at the end when joining, also .Join() does magic things
            int numberOfPresses = presses.Split().Length;
            if (presses.RegexMatch(new string[] { "^(0 ?)*$", @"^0+\d*$", @"\D0$" })) return "sx"; //since this method offers me to input a string, for readability purposes, I won't just mash up all the regexes
            else if (numberOfPresses > chainLimit) return string.Format("sendtochaterror Sorry! You exceeded the number of buttons you can press at a time, which in this case is {0} (you tried to press {1}). I would've striked you, but I feel lazy.\tThe end? Question mark???", chainLimit, numberOfPresses);
            else return presses;
        }
        return "sx";
    }

    //example: "{1}{1}{0}{1}{1}{0}{0}{0}"
    private string GenerateSalt() {
        return Convert.ToString(random.Range(0, 256), 2) //8 bits
            .PadLeft(8, '0')
            .Select(n => "{"+n+"}")
            .ToArray()
            .Join()
            .Replace(" ", "");
    }

#pragma warning disable 414 //created but not used
    IEnumerator ProcessTwitchCommand(string input)
#pragma warning restore 414
    {
        string logMessage = string.Format("You did the command: <<{0}>>. ", input);
        string chainReturnValue = ParseChainCommand(input).Trim();
        if (chainReturnValue[0] != 's')
        {
            logMessage += "(valid)";
            if (chainReturnValue.ToLowerInvariant() == "nothing")
            {
                ModuleLog("You did the \"nothing\" command. u funni person eh");
                yield return string.Format("sendtochat YES! YOU DID NOTHING! WOOHOO!! {0} (well, as if you were actually doing something to solve the module, huh)", GenerateSalt());
            }
            else
            {
                ModuleLog(logMessage);
                if (char.ToLowerInvariant(chainReturnValue[0]) == 'm')
                {
                    if (side == 2)
                    {
                        yield return "sendtochaterror Oop. You are not in ruleseed! I will now proceed to ignore you.";
                        yield return new WaitForSeconds(3f);
                    }
                    else
                    {
                        yield return "sendtochat Okay. Fine. TTwTT.\nThere's no turn back, eh?.";
                        muted = true;
                    }
                }
                else
                {
                    int[] numbers = Array.ConvertAll(chainReturnValue.Split(), Convert.ToInt32);
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        buttons[numbers[i] - 1].OnInteract();
                        if (i != numbers.Length - 1) yield return new WaitForSeconds(0.3f);
                    }
                }
            }

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
#pragma warning disable 414 //created but not used
    IEnumerator TwitchHandleForcedSolve()
#pragma warning restore 414
    {
        if (solved) yield break;
        yield return null;
        if (side == 2)
        {
            DoEasterEgg(true);
            yield return new WaitForSeconds(7f);
        }
        else
        {
            //all fun and jokes until you realise you actually can't solve too many buttons with sound; otherwise, just, earrape
            float[] interactDelays = new float[] { 0.2f, 0.045f, 0.004f }; //careful with these values
            float[] afterInteractDelays = new float[] { 2f, 2.5f, 2.6f };
            bool[] mutes = new bool[] { false, false, true }; //specially these
            int[] sidesThresholds = new int[] { 5, 7 };
            int i = 0;

            for (i = 0; i < 2; i++)
                if (side < sidesThresholds[i]) break; //<<else if>> generalisation
            muted = mutes[i];

            Audio.PlaySoundAtTransform("Lo-hicimos", Module.transform);
            yield return true;

            for (int j = 0; j < 2; j++)
                yield return StartCoroutine(RandomSolved());

            foreach (KMSelectable button in buttons)
            {
                button.OnInteract();
                yield return new WaitForSeconds(interactDelays[i]);
            }
            yield return new WaitForSeconds(afterInteractDelays[i]);

            foreach (KMSelectable button in buttons)
                button.GetComponentInChildren<TextMesh>().color = Color.red;
        }
        foreach (KMSelectable button in buttons)
            button.GetComponentInChildren<TextMesh>().text = "Shame.";

        ModuleLog("Forced solving compelete.");
        M.HandlePass();
    }
}