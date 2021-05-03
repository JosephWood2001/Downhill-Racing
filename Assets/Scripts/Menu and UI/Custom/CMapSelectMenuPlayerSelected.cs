using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMapSelectMenuPlayerSelected : MapSelectMenu
{
    public override void SelectCup(MapCatolog.Cup cup)
    {
        foreach (GameObject shelf in shelfCupsLocations)
        {

            for (int h = 0; h < shelf.transform.childCount; h++)
            {
                Destroy(shelf.transform.GetChild(h).gameObject);
            }
        }

        int i = cup.courses.Length;
        for (int j = 0; j < i; j++)
        {
            GameObject temp = Instantiate(shelfCup, shelfCupsLocations[j].transform);
            temp.GetComponent<ShelfCup>().SetMap(cup,j);
        }


    }


    public override void SelectMap(MapCatolog.Cup cup, int courseId)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().cup = cup;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().courseOfCupId = courseId;
        parentMenu.CurrentMenu = 0;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().LoadPlayerSelected();
    }

    public override void Back()
    {

        //if the loaded course isn't set, then back is valid
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().loadedCourse.Equals(""))
        {
            base.Back();
            parentMenu.CurrentMenu = 7;
        }
        //else, a current game is in session with results and will need to be reset if to go back
        else
        {
            //TODO: Open a pop-up window to ensure the player wants to go back
            //TEMPERARY: Just go back

            GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().loadedCourse = "";
            parentMenu.CurrentMenu = 6;

        }

        
    }
}
