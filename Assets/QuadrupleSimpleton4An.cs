/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using random = UnityEngine.Random;

public class QuadrupleSimpleton4An : MonoBehaviour { my big steps uwu

    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable Button;
    public KMSelectable ModuleSelectable;

    public KMRuleSeedable ruleSeedable;

    private enum Positions { BL, BR, TL, TR };

    private int pressed;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {

        moduleId = moduleIdCounter++;
        Debug.Log("T means Top, B means Bottom, L means Left, R means Right.");
    }

    // Use this for initialization no thx lol
    void Start ()
    {
        int seedRNG = ruleSeedable.GetRNG().Next();
        int n = seedRNG % 9 + 2; //range: [2,inf[ € N ... range I chose: [2, 10]
        Transform ButtonTransform = Button.transform;
        List<KMSelectable> newSelectables = new List<KMSelectable>();

        for (int i = 0; i < n * n; i++)
        {
            Transform clone = Instantiate(ButtonTransform, ButtonTransform.parent);
            clone.transform.localScale = new Vector3(clone.transform.localScale.x / n,
                                                     clone.transform.localScale.y,
                                                     clone.transform.localScale.z / n);
            clone.transform.localPosition = CalculatePositions(i, clone, n);
            newSelectables.Add(clone.GetComponent<KMSelectable>());
        }
        ModuleSelectable.Children = newSelectables.ToArray();
        ModuleSelectable.UpdateChildrenProperly();
        Destroy(Button.gameObject);

        for (int i = 0; i < newSelectables.Count; i++)
        {
            int j = i; //bug lmao
            newSelectables[j].OnInteract += () => { ButtonHandler(j, newSelectables, n); return false; };
        }
    }

    private void ButtonHandler (int position, List<KMSelectable> selectableButtons, int n)
    {
        if (selectableButtons[position].GetComponentInChildren<TextMesh>().text == "VICTORY!")
            { if (n == 2) Debug.Log("You have just pressed the " + (Positions)position + " button again. You should be proud of yourself :3"); }
        else
        {
            PlayButtonAudio(position, selectableButtons);
            pressed++;
            if (n == 2) Debug.Log("You pressed the " + (Positions)position + " button. Congrats!");
            else Debug.Log("You pressed " + pressed + " button" + (pressed != 1 ? "s." : "."));
            selectableButtons[position].GetComponentInChildren<TextMesh>().text = "VICTORY!";
        }

        if (Check4solve(n)) Module.HandlePass();
    }

    private bool Check4solve (int n) { return pressed >= n * n; }

    private void PlayButtonAudio (int position, List<KMSelectable> selectableButtons)
    {
        int magicNumber = random.Range(0, 100);
        if (magicNumber == 0) Audio.PlaySoundAtTransform("Victory", selectableButtons[position].transform);
        else if (magicNumber > 20) Audio.PlaySoundAtTransform("Victory", selectableButtons[position].transform);
        else Audio.PlaySoundAtTransform("ML Victory Sound", selectableButtons[position].transform);
    }

    private Vector3 CalculatePositions (int i, Transform clone, int n)
    { //module boundaries: [0.1, -0.1]
        float margin = 0.2f / n + 0.02f + n / 1000f;
        float distance = 0.2f - margin;
        float widthDistribution = i % n * distance / (n - 1) - 0.1f + margin / 2 + 0.0055f*1/n; //slight off-center corection
        float heightDistribution = i / n * distance / (n - 1) - 0.1f + margin / 2;

        return new Vector3(widthDistribution, clone.localPosition.y, heightDistribution);
    }

    /* i => index (0 to n) of the button
     * n => height/width of the distribution. Recall n*n = total number of buttons
     * 
     * Notice: array2D[i%height,i/width] will display all the elements in array2D
     * I will only center in "widthDistribution", taking "heightDistribution" for granted.
     * 
     * i%n is the index in the row. If you do index/(n-1), you will get all the distances
     * you need to evenly distribute n elements in an area.
     * Since this relationship is a unitary distance, I will multiply it by the distance
     * between the left side and the rigth side of the module, because that is the total
     * length my row will be in.
     * I am assuming the starting point (the left part of the module) is 0, so to get the buttons
     * inside the actual module I will subtract 0.1 from the result.
     * Finally, since I took up a bit of "distance" for making margin, I need to put it back into
     * the calculation so my starting point remains at -0.1, hence margin/2.
     */
/*}*/