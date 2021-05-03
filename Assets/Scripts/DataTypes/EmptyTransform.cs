using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EmptyTransform
{
    
    public Vector3 position;
    public Quaternion rotation;

    public EmptyTransform()
    {
        position = new Vector3(0,0,0);
        rotation = (new Quaternion(0,0,0,0)).normalized;
    }

    public EmptyTransform(Vector3 vector3, Quaternion quaternion)
    {
        position = vector3;
        rotation = quaternion;
    }
}
