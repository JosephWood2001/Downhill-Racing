using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementCursor : MonoBehaviour
{
    public ObjectsLog objLog;
    public LayerMask placeAtopLayer;
    public PartPanel placePartSelected;
    GameObject ghostObj;

    EmptyTransform placePoint = new EmptyTransform(Vector3.zero, Quaternion.identity);
    void Update()
    {
        if (GetCursorWorldPoint() != Vector3.zero)
        {
            
            placePoint.position = GetCursorWorldPoint();
            placePoint = Snapping(placePoint);
            if (ghostObj != null)
            {
                ghostObj.SetActive(true);

            }
            if (placePartSelected != null)
            {
                ghostObj.transform.position = placePoint.position;
                ghostObj.transform.rotation = placePoint.rotation;
            }

            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                if (placePartSelected != null)
                {
                    PlacePart(placePoint);
                }
            }
        }
        else
        {
            if (ghostObj != null)
            {
                ghostObj.SetActive(false);

            }
        }

        
    }

    public float tolerance = .5f;
    public EmptyTransform Snapping(EmptyTransform ghostPos)
    {
        foreach (ObjectsLog.PartInfo partInfo in objLog.GetParts())
        {
            foreach (EmptyTransform emptyTransform in partInfo.part.snapPoints)
            {
                foreach (EmptyTransform thisEmptyTransform in placePartSelected.snapPoints)
                {
                    Debug.Log(emptyTransform.position + partInfo.obj.transform.position + "   " + (thisEmptyTransform.position + ghostPos.position));
                    if ((emptyTransform.position + partInfo.obj.transform.position //placed object's snap point position
                        - (thisEmptyTransform.position + ghostPos.position) //ghost object's snap point position
                        ).magnitude <= tolerance
                        &&
                        Quaternion.Angle(emptyTransform.rotation * partInfo.part.transform.rotation, ////placed object's snap point rotation
                        emptyTransform.rotation * partInfo.part.transform.rotation) ////ghost object's snap point rotation
                        < 10000000000f)
                    {
                        return new EmptyTransform(partInfo.obj.transform.position + emptyTransform.position - thisEmptyTransform.position, emptyTransform.rotation);
                    }
                }
            }
        }
        
        return ghostPos;
    }

    public Vector3 GetCursorWorldPoint()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, 1000f, placeAtopLayer))
            {
                return Vector3.zero;
            }
            return hit.point;
        }
        return Vector3.zero;

    }

    public void PlacePart(EmptyTransform emptyTransform)
    {
        GameObject temp = Instantiate(placePartSelected.partPrefab);
        GameObject.FindGameObjectWithTag("ObjectsLog").GetComponent<ObjectsLog>().Add(new ObjectsLog.PartInfo(placePartSelected,temp));
        temp.transform.position = emptyTransform.position;
        //temp.transform.rotation = emptyTransform.rotation;
    }

    public void PickPart(PartPanel part)
    {
        if (part == placePartSelected)
        {
            placePartSelected = null;
            return;
        }
        placePartSelected = part;
        Destroy(ghostObj);
        ghostObj = Instantiate(part.partPrefab);
        Material mat = ghostObj.GetComponentInChildren<Renderer>().material;
        Color old = mat.color;
        MaterialExtensions.ToFadeMode(mat);
        Color newColor = new Color(old.r, old.g, old.b, old.a / 4f);
        mat.SetColor("_Color",newColor);

        if (ghostObj.GetComponentInChildren<MeshCollider>() != null)
        {
            ghostObj.GetComponentInChildren<MeshCollider>().enabled = false;
        }
    }

    
}
