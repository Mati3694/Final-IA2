using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class BezierUtility
{
    static public Vector3 SimpleBezier(Vector3 initialPos, Vector3 finalPos, float t, float bezierElevation = 1)
    {
        Vector3 middlePos = Vector3.Lerp(initialPos, finalPos, 0.5f);
        middlePos += Vector3.up * bezierElevation;
        Vector3 xy = Vector3.Lerp(initialPos, middlePos, t);
        Vector3 yz = Vector3.Lerp(middlePos, finalPos, t);

        return Vector3.Lerp(xy, yz, t);
    }
}
