using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SKartSelectMenu : Menu
{
    public void BackToMainMenu()
    {
        parentMenu.CurrentMenu = 2;
    }

    public void GoToMapSelectMenu()
    {
        parentMenu.CurrentMenu = 4;
    }

}
