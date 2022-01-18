using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using random = UnityEngine.Random;

public class QuadrupleSimpletonOOP : MonoBehaviour //definitive version :P
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable Button;
    public KMSelectable ModuleSelectable;
    public GameObject StatusLight;

    public KMRuleSeedable ruleSeedable;

    private int presses;
    private int solved;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake ()
    {

        moduleId = moduleIdCounter++;
        Debug.LogFormat("[Quadruple Simpleton #{0}] T means Top, B means Bottom, L means Left, R means Right.", moduleId);
    }

    // Use this for initialization no thx lol
    void Start ()
    {
        ButtonBehaviour thisBehaviour;
        int side = 1;

        if (ruleSeedable.GetRNG().Seed == 1) { side = 2; thisBehaviour = new NormalBehaviour(); }
        else { side = ruleSeedable.GetRNG().Next(8) % 8 + 3; thisBehaviour = new RandomBehaviour(side); StatusLight.SetActive(false); }

        List<KMSelectable> newSelectables = new List<KMSelectable>();

        for (int i = 0; i < side * side; i++)
        {
            Transform clone = Instantiate(Button.transform, Button.transform.parent);
            clone.transform.localScale = thisBehaviour.CalculateSize( clone.transform.localScale.x,
                                                                      clone.transform.localScale.y,
                                                                      clone.transform.localScale.z);
            clone.transform.localPosition = thisBehaviour.CalculatePositions(i, clone.transform.localPosition.y);

            newSelectables.Add(clone.GetComponent<KMSelectable>());
        }
        ModuleSelectable.Children = newSelectables.ToArray();
        ModuleSelectable.UpdateChildrenProperly();
        Destroy(Button.gameObject);

        for (int i = 0; i < newSelectables.Count; i++)
        {
            int j = i; //bug lmao
            newSelectables[j].OnInteract += () => { ButtonHandler(j, newSelectables, thisBehaviour); return false; };
        }
    }

    private void ButtonHandler (int position, List<KMSelectable> selectableButtons, ButtonBehaviour behaviour)
    {
        if (selectableButtons[position].GetComponentInChildren<TextMesh>().text != "PUSH IT!")
        { Debug.LogFormat("[Quadruple Simpleton #{0}] " + behaviour.AgainMessage(position), moduleId); }
        else
        {
            presses++;
            PlayButtonAudio();
            Debug.LogFormat("[Quadruple Simpleton #{0}] " +
                             behaviour.ButtonMessage(behaviour.GetType().Name == "RandomBehaviour" ? presses : position),
                             moduleId);
            selectableButtons[position].GetComponentInChildren<TextMesh>().text = "VICTORY!";
        }

        if (moduleSolved)
        {
            StartCoroutine(ButtonPush(selectableButtons[position].transform));
            return;
        }

        solved = behaviour.Check4solve(presses);
        if (solved > 0)
        {
            Module.HandlePass();
            moduleSolved = true;
            if (solved == 1)
            {
                if (random.Range(0, 100) == 0) //easter egg :P
                {
                    Audio.PlaySoundAtTransform("Lo hicimos", ModuleSelectable.transform);
                    string[] _easterEgg = new string[4] { "¡Lo", "hicimos!", "We did", "it!" };
                    for (int i = 0; i < 4; i++)
                        selectableButtons[i].GetComponentInChildren<TextMesh>().text = _easterEgg[i];
                    Debug.LogFormat("[Quadruple Simpleton #{0}] You did it!! Congrats! :D", moduleId);
                }
            }
            else
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, ModuleSelectable.transform);
                StartCoroutine(RandomSolved(selectableButtons));
            }
        }
    }

    IEnumerator RandomSolved (List<KMSelectable> selectableButtons)
    {
        foreach (KMSelectable button in selectableButtons)
            button.GetComponentInChildren<TextMesh>().color = new Color(0, 200 / 255f, 0);
        yield return new WaitForSeconds(0.4f);
        foreach (KMSelectable button in selectableButtons)
            button.GetComponentInChildren<TextMesh>().color = Color.white;
        yield return new WaitForSeconds(0.4f);

        foreach (KMSelectable button in selectableButtons)
            button.GetComponentInChildren<TextMesh>().color = new Color(0, 200 / 255f, 0);
        yield return new WaitForSeconds(0.4f);
        foreach (KMSelectable button in selectableButtons)
            button.GetComponentInChildren<TextMesh>().color = Color.white;
        yield return new WaitForSeconds(0.4f);

        foreach (KMSelectable button in selectableButtons)
            button.GetComponentInChildren<TextMesh>().color = new Color(0, 200 / 255f, 0);
    }

    IEnumerator ButtonPush (Transform pressedButtonTransform)
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

    private void PlayButtonAudio() { Audio.PlaySoundAtTransform("Victory", ModuleSelectable.transform); }
}