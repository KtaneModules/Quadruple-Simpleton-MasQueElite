using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour //interval for no UB: [2, inf[
{
    private int _n; //no "readonly" here due to the warning. rip
    private int _presses = 0;
    private enum Positions { BL, BR, TL, TR };

    public static void PutConstructorPropierty(ref GameObject workaround, int sideLength) //static method for the workaround: no constructors allowed here
    {
        ButtonBehaviour thisButtonBehaviourInstance = workaround.GetComponent<ButtonBehaviour>(); //GetComponent<T>() returns that ButtonBehaviour component
        thisButtonBehaviourInstance._n = sideLength;
    }

    public Vector3 CalculateSize(float x, float y, float z) { return new Vector3(x / _n, y, z / _n); }

    public Vector3 CalculatePositions(int cloneNumber, float y) //widthDist. and heightDist. are different for the sake of applying two different solutions
    { //module boundaries: [0.1, -0.1]

        float margin = 0.2f / _n + 0.02f + _n / 1000f;
        
        float widthDistribution =
            Mathf.LerpUnclamped( //unclamped because makes any error visual
                -0.1f + margin / 2,
                 0.1f - margin / 2,
                 cloneNumber % _n / (float)(_n - 1));

        float distance = 0.2f - margin;
        float heightDistribution = cloneNumber / _n * distance / (_n - 1) - 0.1f + margin / 2; //magic formula :)
        
        return new Vector3(widthDistribution, y, heightDistribution);
    }

    public string AgainMessage(int cloneNumber)
    {
        if (_n == 2) return string.Format("You have just pressed the {0} button again. You should be proud of yourself :3", (Positions)cloneNumber);
        return string.Format("You pressed button {0} and you changed absolutely nothing. Hooray!", cloneNumber + 1);
    }
    public string ButtonMessage(int cloneNumber)
    {
        if (_n == 2) return string.Format("You pressed the {0} button. Congrats!", (Positions)cloneNumber);
        return string.Format("You pressed {0} button{1}.", _presses + 1, _presses + 1 != 1 ? "s" : "");
    }

    public bool CheckSolve()
    {
        _presses++;
        return _presses >= _n * _n;
    }
}