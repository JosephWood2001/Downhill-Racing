using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKartSelectMenu : Menu
{
    public void BackToCustomGameMenu()
    {
        parentMenu.CurrentMenu = 6;
    }

    public void Continue()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>().BeginSession();
        //BeginSeason will change the menu if nessisary
    }

}
