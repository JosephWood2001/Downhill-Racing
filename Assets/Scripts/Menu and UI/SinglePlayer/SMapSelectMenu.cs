using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMapSelectMenu : MapSelectMenu
{

    
    public override void SelectCup(MapCatolog.Cup cup)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<SGameManager>().cup = cup;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<SGameManager>().courseOfCupId = -1;

        parentMenu.CurrentMenu = 5;
    }

    public override void Back()
    {
        base.Back();
        parentMenu.CurrentMenu = 3;
    }
}
