using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBehaviour : ButtonBehaviour
{

    private readonly int n;

    public RandomBehaviour (int sideLength) { n = sideLength; }

    public override Vector3 CalculateSize (float x, float y, float z) { return new Vector3(x / n, y, z / n); }

    public override Vector3 CalculatePositions (int cloneNumber, float y)
    { //module boundaries: [0.1, -0.1]

        if (n < 0) return new Vector3(-1f, -2f, -1f);

        float margin = 0.2f / n + 0.02f + n / 1000f;
        float distance = 0.2f - margin;                                                     //slight off-center correction
        float widthDistribution = cloneNumber % n * distance / (n - 1) - 0.1f + margin / 2 + 0.0055f * 1 / n;
        float heightDistribution = cloneNumber / n * distance / (n - 1) - 0.1f + margin / 2;

        return new Vector3(widthDistribution, y, heightDistribution);
    }

    public override string AgainMessage (int cloneNumber) { return "You changed absolutely nothing."; }
    public override string ButtonMessage (int presses) { return "You pressed " + presses + " button" + (presses != 1 ? "s." : "."); }

    public override int Check4solve (int presses) {

        return presses >= (n * n) ? 2 : 0;
    }
}