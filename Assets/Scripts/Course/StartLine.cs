using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLine : MonoBehaviour
{

    public EmptyTransform[] emptyTransforms = new EmptyTransform[8];

    public void SetStartPos(int index, Kart kart)
    {
        EmptyTransform emptyTransform = GetStartPos(index);
        kart.transform.position = emptyTransform.position;
        kart.transform.rotation = emptyTransform.rotation;
    }

    public EmptyTransform GetStartPos(int index)
    {
        if(index > emptyTransforms.Length - 1)
        {
            Debug.Log("More Karts then starting position");
            return new EmptyTransform(transform.position + transform.rotation * emptyTransforms[index % 8].position + Vector3.up * index / 8, transform.rotation * emptyTransforms[index % 8].rotation.normalized);
        }
        return new EmptyTransform(transform.position + transform.rotation * emptyTransforms[index].position, transform.rotation * emptyTransforms[index].rotation.normalized);
    }


    private void OnDrawGizmos()
    {
        EmptyTransform startPos;
        for (int i = 0; i < emptyTransforms.Length; i++)
        {
            startPos = GetStartPos(i);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPos.position, .2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos.position, startPos.position + startPos.rotation * Vector3.forward);
            
        }
        
    }
}
