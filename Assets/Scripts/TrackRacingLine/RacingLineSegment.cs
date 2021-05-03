using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RacingLineSegment
{
    public Vector3 point1;
    public Vector3 point2;

    public RacingLineSegment(Vector3 point1, Vector3 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }

}
