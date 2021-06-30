using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PartPicker : MonoBehaviour
{
    public GameObject content;

    [Header("Buttons")]
    public Button trackBtn;

    [Header("Category contents")]
    public GameObject[] trackObjs;

    List<GameObject> objsInContent;

    public PlacementCursor placementCursor;
    private void Start()
    {
        objsInContent = new List<GameObject>();
        trackBtn.onClick.AddListener(() => { Repopulate(trackObjs); });

    }

    public void Test()
    {

    }

    public void Repopulate(GameObject[] gameObjects)
    {
        //unpopulate
        foreach (GameObject obj in objsInContent)
        {
            Destroy(obj);
        }
        objsInContent = new List<GameObject>();


        //populate
        foreach (GameObject obj in gameObjects)
        {
            GameObject temp = Instantiate(obj, content.transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, objsInContent.Count * -200);
            objsInContent.Add( temp );
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, objsInContent.Count * 200);
        }

    }
    

    
}
