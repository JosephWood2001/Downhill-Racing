using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CCustomGameMenu : Menu
{
    [Header("Refrences")]
    public TextMeshProUGUI numOfRacersText;
    public TextMeshProUGUI numOfSeasonRacesText;
    public TextMeshProUGUI randMSM;
    public TextMeshProUGUI selfSelectMSM;
    public TextMeshProUGUI selectCupMSM;
    public TextMeshProUGUI easy;
    public TextMeshProUGUI medium;
    public TextMeshProUGUI hard;
    public TextMeshProUGUI[] greyForCupMapSelectMode;

    int numberOfRacers;
    public int NumberOfRacers
    {
        get
        {
            return numberOfRacers;
        }
        set
        {
            numberOfRacers = value;
            numOfRacersText.text = numberOfRacers.ToString();
        }
    }

    public void NumberOfRacerDecrement()
    {
        // 1 and 8 clamp the max and min numbers of players
        NumberOfRacers = Mathf.Clamp(NumberOfRacers - 1, 1, 8);
    }

    public void NumberOfRacerIncrement()
    {
        // 1 and 8 clamp the max and min numbers of players
        NumberOfRacers = Mathf.Clamp(NumberOfRacers + 1, 1, 8);
    }

    int mapSelectMode;
    public int MapSelectMode
    {
        get
        {
            return mapSelectMode;
        }
        set
        {
            
            if (mapSelectMode == 0)
            {
                randMSM.color = Color.white;
            }
            else if (mapSelectMode == 1)
            {
                selfSelectMSM.color = Color.white;
            }
            else if (mapSelectMode == 2)
            {
                selectCupMSM.color = Color.white;
            }
            mapSelectMode = value;
            if(mapSelectMode == 0)
            {
                randMSM.color = Color.yellow;
                UngreyForCupMapSelectMode();
            }
            else if(mapSelectMode == 1)
            {
                selfSelectMSM.color = Color.yellow;
                UngreyForCupMapSelectMode();
            }
            else if (mapSelectMode == 2)
            {
                selectCupMSM.color = Color.yellow;
                GreyForCupMapSelectMode();
            }
        }
    }

    int numOfSeasonRaces;
    public int NumOfSeasonRaces
    {
        get
        {
            return numOfSeasonRaces;
        }
        set
        {
            numOfSeasonRaces = value;
            numOfSeasonRacesText.text = numOfSeasonRaces.ToString();
        }
    }

    public void NumberOfRacesDecrement()
    {
        // 1 and 99 clamp the max and min numbers of players
        NumOfSeasonRaces = Mathf.Clamp(NumOfSeasonRaces - 1, 1, 99);
    }

    public void NumberOfRacesIncrement()
    {
        // 1 and 99 clamp the max and min numbers of players
        NumOfSeasonRaces = Mathf.Clamp(NumOfSeasonRaces + 1, 1, 99);
    }

    int difficulty;
    public int Difficulty
    {
        get
        {
            return difficulty;
        }
        set
        {
            if (difficulty == 0)
            {
                easy.color = Color.white;
            }
            else if (difficulty == 1)
            {
                medium.color = Color.white;
            }
            else if (difficulty == 2)
            {
                hard.color = Color.white;
            }
            difficulty = value;
            if (difficulty == 0)
            {
                easy.color = Color.yellow;
            }
            else if (difficulty == 1)
            {
                medium.color = Color.yellow;
            }
            else if (difficulty == 2)
            {
                hard.color = Color.yellow;
            }
        }
    }

    private void Start()
    {
        NumberOfRacers = 8;
        MapSelectMode = 0;
        NumOfSeasonRaces = 4;
        Difficulty = 1;
    }

    private void SetSettingsFromGameManager()//Might not use this
    {
        CGameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>();
        NumberOfRacers = gameManager.numberOfRacers;
        MapSelectMode = gameManager.mapSelectMode;
        NumOfSeasonRaces = gameManager.numOfSeasonRaces;
    }

    public void SetSettingsAndTooKartSelect()
    {
        CGameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CGameManager>();
        gameManager.Initialize(NumberOfRacers, MapSelectMode, NumOfSeasonRaces);
        parentMenu.CurrentMenu = 7;
    }

    void GreyForCupMapSelectMode()
    {
        foreach (TextMeshProUGUI text in greyForCupMapSelectMode)
        {
            text.color = Color.grey;
        }
    }

    void UngreyForCupMapSelectMode()
    {
        foreach (TextMeshProUGUI text in greyForCupMapSelectMode)
        {
            text.color = Color.white;
        }
    }

    public void BackToMainMenu()
    {
        parentMenu.CurrentMenu = 2;
    }
}
