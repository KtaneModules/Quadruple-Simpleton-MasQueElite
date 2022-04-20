using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using random = UnityEngine.Random;

//note: all the fields/methods I'm not commenting should be auto-documentable
public class QuadrupleSimpleton : MonoBehaviour
{
    public KMBombModule M;
    public KMAudio Audio;
    public KMSelectable RefButton;
    public KMSelectable Module;
    public GameObject StatusLight;
        public KMRuleSeedable RuleSeed;
        private int seed;

    private bool solved;
    private GameObject workaround;
    private ButtonBehaviour behaviour;
    private List<KMSelectable> buttons = new List<KMSelectable>();

    int moduleId;

    void Awake()
    {
        Debug.LogFormat("[Quadruple Simpleton #{0}] T means Top, B means Bottom, L means Left, R means Right.", moduleId);
        moduleId++;
        seed = RuleSeed.GetRNG().Seed;
        int side;
        makeBehaviourDecision(out side);
        WorkaroundTheWarning(side);
        MakeButtons(side);
    }
    //workaround for the warning: "You are trying to create a MonoBehaviour using the 'new' keyword. This is not allowed. MonoBehaviours can only be added using AddComponent()."
    private void WorkaroundTheWarning(int side)
    {
        workaround = new GameObject();
        workaround.AddComponent<ButtonBehaviour>();
        ButtonBehaviour.PutConstructorPropierty(ref workaround, side);
        behaviour = workaround.GetComponent<ButtonBehaviour>();
    }

    
    //TODO: REMOVE UNUSED LIBRARIES
    //still the workaround
    //"outside" lol :^
    //TODO: CHECK MATF.LERP
    //every module has to have an ID number
    //note: GetRNG just returns a MonoRandom which has the property I need ("Seed").
    //how many buttons will the NxN square have?

    /* It does three things: (based off "seed")
     * - Returns "side" with value
     * - Modifies the status light (to be visible or not)
     * - Sends an extra output message
     */
    private void makeBehaviourDecision(out int side)
    {
        if (seed == 1)
            side = 2;
        else
        {
            side = RuleSeed.GetRNG().Next(8) % 9 + 3; //Interval: [3, 11]
            Debug.LogFormat("[Quadruple Simpleton #{0}] Button order is from top to bottom, right to left.", moduleId);
            StatusLight.SetActive(false);
        }
    }

    private void HookButtons(List<KMSelectable> buttons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int j = i; //bug lmao TODO: CHECK THIS
            buttons[j].OnInteract += () => ButtonHandler(buttons[j], j);
        }
    }

    private void MakeButtons(int side)
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

    private void PlayButtonAudio() { Audio.PlaySoundAtTransform("Victory", Module.transform); }
    private void DoEasterEgg()
    {
        Audio.PlaySoundAtTransform("Lo-hicimos", Module.transform);
        buttons[0].GetComponentInChildren<TextMesh>().text = "¡Lo";
        buttons[1].GetComponentInChildren<TextMesh>().text = "hicimos!";
        buttons[2].GetComponentInChildren<TextMesh>().text = "We did";
        buttons[3].GetComponentInChildren<TextMesh>().text = "it!";
        Debug.LogFormat("[Quadruple Simpleton #{0}] You did it!! Congrats! :D", moduleId);
    }
    private bool ButtonHandler(KMSelectable button, int position)
    {
        //just an abstraction
        bool pressed = button.GetComponentInChildren<TextMesh>().text != "PUSH IT!";

        if (pressed)
        {
            Debug.LogFormat("[Quadruple Simpleton #{0}] {1}", moduleId,
                             behaviour.AgainMessage(position));
            Audio.PlaySoundAtTransform("boing", button.transform);
            //already been solved?
            if (solved) button.AddInteractionPunch(100f);
            else StartCoroutine(ButtonPush(button.transform));
        }
        else
        {
            PlayButtonAudio();
            button.AddInteractionPunch();
            Debug.LogFormat("[Quadruple Simpleton #{0}] {1}", moduleId,
                             behaviour.ButtonMessage(position));
            button.GetComponentInChildren<TextMesh>().text = "VICTORY!";
            //not solved? check
            solved = behaviour.CheckSolve();
            if (solved)
            {
                M.HandlePass();
                if (1 == 1) //TODO: SEED == 1
                { if (random.Range(0, 50) == 0) DoEasterEgg(); }
                //else StartCoroutine(RandomSolved());
            }
        }

        return false;
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

    IEnumerator ButtonPush(Transform pressedButtonTransform) //if you jitter click you can sink the button
    {
        pressedButtonTransform.localPosition =
            new Vector3(pressedButtonTransform.localPosition.x,
                        pressedButtonTransform.localPosition.y - 0.0015f,
                        pressedButtonTransform.localPosition.z);
        yield return new WaitForSeconds(0.2f);
        pressedButtonTransform.localPosition =
            new Vector3(pressedButtonTransform.localPosition.x,
                        pressedButtonTransform.localPosition.y + 0.0015f,
                        pressedButtonTransform.localPosition.z);
    }
}