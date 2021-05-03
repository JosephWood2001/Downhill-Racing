using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//works as maps too
public class ShelfCup : MonoBehaviour
{
    private MapCatolog.Cup cup;
    private int courseId = -1;
    bool isCup = true;//true is cup; false is map

    public void SetCup(MapCatolog.Cup cup)
    {
        this.cup = cup;
        Instantiate(cup.cupMiniIcon, gameObject.transform);
        
    }

    public void SetMap(MapCatolog.Cup cup, int courseId)
    {
        isCup = false;
        this.cup = cup;
        this.courseId = courseId;
        Instantiate(cup.coursesMiniIcons[courseId], gameObject.transform);

    }

    public void SelectThisCup()
    {
        if (isCup)
        {
            gameObject.GetComponentInParent<MapSelectMenu>().SelectCup(cup);
        }
        else //is Map
        {
            gameObject.GetComponentInParent<MapSelectMenu>().SelectMap(cup,courseId);
        }
        
    }
}
