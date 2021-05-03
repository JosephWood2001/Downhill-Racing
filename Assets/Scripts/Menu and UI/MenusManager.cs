using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusManager : MonoBehaviour
{

    /*
     * 0: no menu
     * 1: settings / pause
     * 
     * 
     */

    private int currentMenu;

    public int CurrentMenu
    {
        get
        {
            return currentMenu;
        }
        set
        {
            if (value != currentMenu)
            {
                value = Mathf.Clamp(value, 0, menus.Length - 1);
                menus[currentMenu].Deactivate();
                menus[value].Activate();
                currentMenu = value;
                if (menus[currentMenu].hasLocation)
                {
                    Camera.main.GetComponent<MainMenuCameraController>().ObjectToFollow = GameObject.FindGameObjectWithTag(menus[currentMenu].tagName).transform;
                }
            }

        }
    }

    public Menu[] menus;

    public virtual void InitForMainMenuLoaded()
    {
        CurrentMenu = 2;

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i] == null)
            {
                continue;
            }

            menus[i].parentMenu = this;

            if (i == currentMenu)
            {
                menus[i].Activate();
            }
            else
            {
                menus[i].Deactivate();
            }
        }
    }


    private void Update()
    {
        
        if (Input.GetKeyDown("y"))
        {
            CurrentMenu = 1;
        }
    }
}
