/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBehaviour : IButtonBehaviour --- Non-rule-seed behaviour
{
    public int id { get; private set; }
    private readonly int _n = 2;
    private int _presses = 0;

    public RandomBehaviour (int sideLength) { _n = sideLength; id = 2; }

    public Vector3 CalculateSize (float x, float y, float z) { return new Vector3(x / _n, y, z / _n); }

    public Vector3 CalculatePositions (int cloneNumber, float y)
    { //module boundaries: [0.1, -0.1]

        if (_n < 0) return new Vector3(-1f, -2f, -1f);

        float margin = 0.2f / _n + 0.02f + _n / 1000f;
        float distance = 0.2f - margin;                                                     //slight off-center correction
        float widthDistribution = cloneNumber % _n * distance / (_n - 1) - 0.1f + margin / 2 + 0.0055f * 1 / _n;
        float heightDistribution = cloneNumber / _n * distance / (_n - 1) - 0.1f + margin / 2;

        return new Vector3(widthDistribution, y, heightDistribution);
    }

    public string AgainMessage (int cloneNumber) { return string.Format("You pressed button {0} and you changed absolutely nothing. Hooray!", cloneNumber); }
    public string ButtonMessage (int presses) { return "You pressed " + presses + " button" + (presses != 1 ? "s." : "."); }

    public bool CheckSolve () {
        _presses++;
        return _presses >= (_n * _n);
    }
}*/