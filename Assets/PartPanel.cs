using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPanel : MonoBehaviour
{
    public GameObject partPrefab;
    public EmptyTransform[] snapPoints;

    public void OnClick()
    {
        GameObject.FindGameObjectWithTag("PlacementCursor").GetComponent<PlacementCursor>().PickPart(this);
    }
}
