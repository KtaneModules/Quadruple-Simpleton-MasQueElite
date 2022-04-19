using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonBehaviour
{
    int id { get; }
    bool CheckSolve();
    string AgainMessage(int cloneNumber);
    string ButtonMessage(int clonePresses);
    Vector3 CalculatePositions(int cloneNumber, float y);
    Vector3 CalculateSize(float x, float y, float z);
}