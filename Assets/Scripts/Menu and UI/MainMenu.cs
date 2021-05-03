using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public GameObject sGameManager;
    public GameObject cGameManager;
    public void SinglePlayer()
    {
        Instantiate(sGameManager,GameObject.FindGameObjectWithTag("GameTypeManagers").transform);
        parentMenu.CurrentMenu = 3;
    }

    public void Custom()
    {
        Instantiate(cGameManager, GameObject.FindGameObjectWithTag("GameTypeManagers").transform);
        parentMenu.CurrentMenu = 6;
    }

    public void ExitToDesktop()
    {
        //Close the game
    }

    public override void Activate()
    {
        base.Activate();
        Transform gameTypeManagerParent = GameObject.FindGameObjectWithTag("GameTypeManagers").transform;

        for (int i = 0; i < gameTypeManagerParent.childCount; i++)
        {
            Destroy(gameTypeManagerParent.GetChild(i).gameObject);
        }
    }

}
