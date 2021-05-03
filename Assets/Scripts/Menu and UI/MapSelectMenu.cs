using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectMenu : Menu
{

    public GameObject[] shelfCupsLocations;
    public GameObject shelfCup;

    public override void Activate()
    {
        int i = GameObject.FindGameObjectWithTag("MapCatolog").GetComponent<MapCatolog>().cups.Length;
        for (int j = 0; j < i; j++)
        {
            GameObject temp = Instantiate(shelfCup, shelfCupsLocations[j].transform);
            temp.GetComponent<ShelfCup>().SetCup(GameObject.FindGameObjectWithTag("MapCatolog").GetComponent<MapCatolog>().cups[j]);
        }

        base.Activate();
    }

    public virtual void SelectCup(MapCatolog.Cup cup)
    {

    }

    public virtual void SelectMap(MapCatolog.Cup cup, int courseId)
    {

    }

    public virtual void Back()
    {

    }

    public override void Deactivate()
    {
        foreach (GameObject shelf in shelfCupsLocations)
        {

            for (int i = 0; i < shelf.transform.childCount; i++)
            {
                Destroy(shelf.transform.GetChild(i).gameObject);
            }
        }
        base.Deactivate();
    }
}
