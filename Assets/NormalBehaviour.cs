using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBehaviour : IButtonBehaviour
{
    public int id { get; private set; }
    private int _presses = 0;

    private readonly float[] positions = new float[4] { -0.039f, 0.039f, -0.03625f, 0.04175f };
    private enum Positions { BL, BR, TL, TR };

    public Vector3 CalculateSize (float x, float y, float z) { return new Vector3(x / 2, y, z / 2); }

    public Vector3 CalculatePositions (int cloneNumber, float y)
    {
        //return new Vector3(positions[cloneNumber % 2 + 2], y, positions[Mathf.FloatToHalf(Mathf.Atan(cloneNumber))]);
        switch (cloneNumber)
        {
            case 0: return new Vector3(positions[0 + 2], y, positions[0]);
            case 1: return new Vector3(positions[1 + 2], y, positions[0]);
            case 2: return new Vector3(positions[0 + 2], y, positions[1]);
            case 3: return new Vector3(positions[1 + 2], y, positions[1]);
        }

        return new Vector3(100f, 100f, 100f);
    }

    public string AgainMessage (int cloneNumber) {
        return "You have just pressed the " +
                (Positions)cloneNumber +
               " button again. You should be proud of yourself :3";
    }

    public string ButtonMessage (int cloneNumber)
    {
        return "You pressed the " +
                (Positions)cloneNumber +
               " button. Congrats!";
    }

    public bool CheckSolve() {
        _presses++;
        return _presses >= 4;
    }
}