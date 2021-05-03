using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDifficultyMenu : Menu
{
    
    public void BeginGame()
    {
        parentMenu.CurrentMenu = 0;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<SGameManager>().BeginGame();
    }

    public void SelectDifficulty(int difficulty)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<SGameManager>().difficulty = difficulty;

    }

    public void Easy()
    {
        SelectDifficulty(0);
    }

    public void Medium()
    {
        SelectDifficulty(1);
    }

    public void Hard()
    {
        SelectDifficulty(2);
    }

    public void Back()
    {
        parentMenu.CurrentMenu = 4;
    }
}
