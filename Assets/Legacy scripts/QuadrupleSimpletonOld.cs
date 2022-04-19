/*using UnityEngine;
using random = UnityEngine.Random;

public class QuadrupleSimpleton : MonoBehaviour { my first steps into this project

    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMSelectable[] Buttons;

    private enum Positions { TL, TR, BL, BR };

    private bool[] pressed = new bool[4]; //defaults to false
    /* I preferred this solution since using...
         Buttons[j].transform.parent.GetComponentInChildren<TextMesh>().text
       ...to check every time if a button has been pressed or not costs more power than acceding to "pressed".
       Nonetheless, making this array costs more space than they array-less solution, naturally.
       While modding, from [power vs space] I will always decide to sacrifice space instead of space.*//*

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () { //0th frame

        moduleId = moduleIdCounter++;
        Debug.Log("T means Top, B means Bottom, L means Left, R means Right.");

        for (int i = 0; i < 4; i++)
        {
            int j = i; //bug lmao
            Buttons[i].OnInteract += () => { ButtonHandler(j); return false; };
        }
    }

    private void ButtonHandler (int position)
    {
        Buttons[position].transform.parent.GetComponentInChildren<TextMesh>().text = "VICTORY!";

        if (pressed[position]) Debug.Log("You have just pressed the " + (Positions)position + " button again. You should be proud of yourself :3");
        else				 { Debug.Log("You pressed the " + (Positions)position + " button. Congrats!"); PlayButtonAudio(position); }

        pressed[position] = true; //sets that button to the pressed state

        if (Check4solve()) Module.HandlePass();
    }

    private bool Check4solve() //plays audio xd
    {
        foreach (bool log in pressed) if (!log) return false;

        /*(else)*//* return true;
    }

    private void PlayButtonAudio(int position) //checks for solved conditions 
    {
        int magicNumber = random.Range(0, 100);
        if (magicNumber == 0) Audio.PlaySoundAtTransform("Lo hicimos", Buttons[position].transform);
        else if (magicNumber > 20) Audio.PlaySoundAtTransform("Victory", Buttons[position].transform);
        else Audio.PlaySoundAtTransform("ML Victory Sound", Buttons[position].transform);
    }
}
*/