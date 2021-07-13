using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartEditPanel : MonoBehaviour
{
    public TMP_InputField posX;
    public TMP_InputField posY;
    public TMP_InputField posZ;

    PlacementCursor placementCursor;
    private void Awake()
    {
        placementCursor = GameObject.FindGameObjectWithTag("PlacementCursor").GetComponent<PlacementCursor>();
    }

    private void OnEnable()
    {
        SetupDisplay();
        UpdateDisplay();
    }

    private void FixedUpdate()
    {
        UpdateDisplay();

    }

    public void SetupDisplay()
    {
        ((TextMeshProUGUI)posX.placeholder).text = "x";
        ((TextMeshProUGUI)posY.placeholder).text = "y";
        ((TextMeshProUGUI)posZ.placeholder).text = "z";
        posX.text = "";
        posY.text = "";
        posZ.text = "";

    }

    public void UpdateDisplay()
    {

        if (placementCursor.selectedPart != null)
        {

            DisplayPosition(placementCursor.selectedPart.obj.transform.position);
            FetchSetPos(placementCursor.selectedPart.obj);
        }
    }

    public void DisplayPosition(Vector3 pos)
    {
        ((TextMeshProUGUI)posX.placeholder).text = "" + pos.x;
        ((TextMeshProUGUI)posY.placeholder).text = "" + pos.y;
        ((TextMeshProUGUI)posZ.placeholder).text = "" + pos.z;

    }

    public Vector3 FetchSetPos(GameObject obj)
    {
        Vector3 pos = new Vector3();
        if (posX.text != "")
        {
            pos.x = float.Parse(posX.text);

        }
        else
        {
            pos.x = obj.transform.position.x;
        }

        if (posY.text != "")
        {
            pos.y = float.Parse(posY.text);

        }
        else
        {
            pos.y = obj.transform.position.y;
        }

        if (posZ.text != "")
        {
            pos.z = float.Parse(posZ.text);

        }
        else
        {
            pos.z = obj.transform.position.z;
        }


        obj.transform.position = pos;
        return pos;

    }
}
