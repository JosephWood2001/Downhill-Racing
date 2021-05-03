using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMapSelectMenuSelectedCup : MapSelectMenu
{
    public override void SelectCup(MapCatolog.Cup cup)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().cup = cup;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().courseOfCupId = -1;

        parentMenu.CurrentMenu = 0;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().BeginSeason();
    }


    public override void Back()
    {
        base.Back();
        parentMenu.CurrentMenu = 7;
    }
}
