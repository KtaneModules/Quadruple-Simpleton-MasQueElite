using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour //interval for no UB: [2, inf[
{
    [SerializeField]
    private int _n; //no "readonly" here due to the warning. rip
    private int _presses = 0;
    private enum Positions { BL, BR, TL, TR };
    
    public static ButtonBehaviour InstantiateProperly(GameObject go, int sideLength) //workaround for "Warning: You are trying to create a MonoBehaviour using the 'new' keyword. This is not allowed. MonoBehaviours can only be added using AddComponent()."
    {
        ButtonBehaviour bb = go.GetComponent<ButtonBehaviour>(); //GetComponent<T>() returns that ButtonBehaviour component
        bb._n = sideLength;

        return bb;
    }
    public static void putPropierty(ref GameObject go, int sideLength) //workaround for "Warning: You are trying to create a MonoBehaviour using the 'new' keyword. This is not allowed. MonoBehaviours can only be added using AddComponent()."
    {
        ButtonBehaviour bb = go.GetComponent<ButtonBehaviour>(); //GetComponent<T>() returns that ButtonBehaviour component
        bb._n = sideLength;
    }

    public ButtonBehaviour(int sideLength) { _n = sideLength; }

    public Vector3 CalculateSize(float x, float y, float z) { return new Vector3(x / _n, y, z / _n); }

    public Vector3 CalculatePositions(int cloneNumber, float y) //TODO: CHECK THIS
    { //module boundaries: [0.1, -0.1]

        if (_n < 0) return new Vector3(-1f, -2f, -1f);

        float margin = 0.2f / _n + 0.02f + _n / 1000f;
        float distance = 0.2f - margin;                                                     //slight off-center correction
        float widthDistribution = cloneNumber % _n * distance / (_n - 1) - 0.1f + margin / 2 + 0.0055f * 1 / _n;
        float heightDistribution = cloneNumber / _n * distance / (_n - 1) - 0.1f + margin / 2;

        return new Vector3(widthDistribution, y, heightDistribution);
    }

    public string AgainMessage(int cloneNumber)
    {
        if (_n == 2) return string.Format("You have just pressed the {0} button again. You should be proud of yourself :3", (Positions)cloneNumber);
        return string.Format("You pressed button {0} and you changed absolutely nothing. Hooray!", _n * _n - cloneNumber);
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