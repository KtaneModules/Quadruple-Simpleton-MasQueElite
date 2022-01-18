using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonBehaviour : QuadrupleSimpletonOOP {

    public abstract Vector3 CalculateSize (float x, float y, float z);
    public abstract Vector3 CalculatePositions (int cloneNumber, float y);
    public abstract string AgainMessage (int cloneNumber);
    public abstract string ButtonMessage (int cloneNumberOrPresses);
    public abstract int Check4solve (int presses);
}