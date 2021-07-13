using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementCursor : MonoBehaviour
{
    public ObjectsLog objLog;
    public LayerMask placeAtopLayer;
    public PartPanel placePartSelected;
    public ObjectsLog.PartInfo selectedPart;
    GameObject ghostObj;

    EmptyTransform placePoint = new EmptyTransform(Vector3.zero, Quaternion.identity);
    void Update()
    {
        if (GetCursorWorldPoint() != Vector3.zero)
        {
            
            placePoint.position = GetCursorWorldPoint();
            if (placePartSelected != null)
            {
                placePoint = Snapping(placePoint);

            }
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
                }else if (GetCursorPart() != null)
                {
                    SelectPart(GetCursorPart());
                }
                else
                {
                    SelectPart(selectedPart); // deselect Part
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
            foreach (EmptyTransform placedSnapPoint in partInfo.part.snapPoints)
            {
                foreach (EmptyTransform placingSnapPoint in placePartSelected.snapPoints)
                {
                    if ((partInfo.obj.transform.rotation * placedSnapPoint.position + partInfo.obj.transform.position //placed object's snap point position
                        - (partInfo.obj.transform.rotation * placedSnapPoint.rotation * Quaternion.Inverse(placingSnapPoint.rotation) * placingSnapPoint.position + ghostPos.position) //ghost object's snap point position
                        ).magnitude <= tolerance
                        &&
                        Quaternion.Angle(placedSnapPoint.rotation * partInfo.part.transform.rotation, //placed object's snap point rotation
                        placedSnapPoint.rotation * partInfo.part.transform.rotation) //ghost object's snap point rotation
                        < 10000000000f)
                    {
                        return new EmptyTransform(partInfo.obj.transform.position + partInfo.obj.transform.rotation * placedSnapPoint.position - partInfo.obj.transform.rotation * placedSnapPoint.rotation * Quaternion.Inverse(placingSnapPoint.rotation) * placingSnapPoint.position,
                            partInfo.obj.transform.rotation * placedSnapPoint.rotation * Quaternion.Inverse(placingSnapPoint.rotation));
                        
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

    public ObjectsLog.PartInfo GetCursorPart()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, 1000f, placeAtopLayer))
            {
                return null;
            }

            foreach(ObjectsLog.PartInfo partInfo in objLog.GetParts())
            {
                if(hit.collider.gameObject.transform.parent != null)
                    if (partInfo.obj == hit.collider.gameObject || partInfo.obj == hit.collider.gameObject.transform.parent.gameObject)
                {
                    return partInfo;
                }
            }
        }
        return null;

    }

    public void PlacePart(EmptyTransform emptyTransform)
    {
        GameObject temp = Instantiate(placePartSelected.partPrefab);
        GameObject.FindGameObjectWithTag("ObjectsLog").GetComponent<ObjectsLog>().Add(new ObjectsLog.PartInfo(placePartSelected,temp));
        temp.transform.position = emptyTransform.position;
        temp.transform.rotation = emptyTransform.rotation;
    }

    public void PickPart(PartPanel part)
    {
        if(selectedPart != null)
        {
            selectedPart = null;
            GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(false);

        }

        if (part == placePartSelected)
        {
            placePartSelected = null;
            GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(true);
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

    public void SelectPart(ObjectsLog.PartInfo partInfo)
    {
        if (partInfo == selectedPart)
        {
            GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(false);//disable BEFORE deselecting part
            selectedPart = null;
            return;
        }
        GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(false);//disable BEFORE selecting part
        selectedPart = partInfo;
        GameObject.FindGameObjectWithTag("PartEdit").transform.GetChild(0).gameObject.SetActive(true);//enable AFTER selecting part
    }

    
}
