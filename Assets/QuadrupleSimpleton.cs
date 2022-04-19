using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using random = UnityEngine.Random;

//before Awake: abstracted utils (usually short); after awake: functionality methods (usually long)
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
    private List<KMSelectable> buttons = new List<KMSelectable>();
    private IButtonBehaviour behaviour;

    int moduleId;

    /* It does three things: (based off "seed")
     * - Instanciates a behaviour and then returns it
     * - Assigns "side" a value
     * - Modifies the status light. (to be visible or not)
     */
    private IButtonBehaviour makeBehaviourDecision(out int side)
    {
        IButtonBehaviour behaviour;
        if (seed == 1)
        {
            side = 2;
            behaviour = new NormalBehaviour();
        }
        else
        {
            side = RuleSeed.GetRNG().Next(8) % 9 + 3; //Interval: [3, 11]
            behaviour = new RandomBehaviour(side);
            StatusLight.SetActive(false);
        }
        return behaviour;
    }

    private void HookButtons(List<KMSelectable> buttons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int j = i; //bug lmao TODO: CHECK THIS
            buttons[j].OnInteract += () => ButtonHandler(buttons[j], j);
        }
    }

    //TODO: ADD THICC INTERACTION PUNCH
    //TODO: REMOVE UNUSED LIBRARIES
    void Awake ()
    {
        //every module has to have an ID number
        moduleId++;
        //how many buttons will the NxN square have?
        int side;
        //note: GetRNG just returns a MonoRandom which has the property I need ("Seed").
        seed = RuleSeed.GetRNG().Seed;
        
        behaviour = makeBehaviourDecision(out side); //"outside" lol :^

        for (int i = 0; i < side * side; i++)
        {
            Transform clone = Instantiate(RefButton.transform, RefButton.transform.parent);
            clone.transform.localScale =
                behaviour.CalculateSize(
                    clone.transform.localScale.x,
                    clone.transform.localScale.y,
                    clone.transform.localScale.z);
            clone.transform.localPosition =
                behaviour.CalculatePositions (
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
            //already been solved?
            if (solved)
                StartCoroutine(ButtonPush(button.transform));
        }
        else
        {
            PlayButtonAudio();
            Debug.LogFormat("[Quadruple Simpleton #{0}] {1}", moduleId,
                             behaviour.ButtonMessage(position));
            button.GetComponentInChildren<TextMesh>().text = "VICTORY!";
            solved = behaviour.CheckSolve(); //not solved? check
            if (solved)
            {
                M.HandlePass();
                if (seed == 1)
                { if (random.Range(0, 50) == 0) DoEasterEgg(); }
                else StartCoroutine(RandomSolved());
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

    IEnumerator ButtonPush(Transform pressedButtonTransform)
    {
        pressedButtonTransform.localPosition = new Vector3(pressedButtonTransform.localPosition.x,
                                                           pressedButtonTransform.localPosition.y - 0.0015f,
                                                           pressedButtonTransform.localPosition.z);
        Audio.PlaySoundAtTransform("boing", pressedButtonTransform);
        yield return new WaitForSeconds(0.2f);
        pressedButtonTransform.localPosition = new Vector3(pressedButtonTransform.localPosition.x,
                                                           pressedButtonTransform.localPosition.y + 0.0015f,
                                                           pressedButtonTransform.localPosition.z);
    }
}