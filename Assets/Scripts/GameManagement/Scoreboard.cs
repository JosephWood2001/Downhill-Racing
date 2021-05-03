using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    private int currentMenu = 0;
    public ScoreboardContentManager scoreboardContent;

    protected virtual void Start()
    {
        scoreboardContent = GameObject.FindGameObjectWithTag("ScoreboardContentManager").GetComponent<ScoreboardContentManager>();
        scoreboardContent.scoreboard = this;
    }

    protected RacerId[] racers; //for casheing, not long term storage

    public virtual void DisplayScoreboardMenu(RacerId[] racers)
    {
        Debug.Log("Test1");
        this.racers = racers;
        scoreboardContent.content.SetActive(true);

        CurrentMenu = 0;
    }

    protected int CurrentMenu
    {
        get
        {
            return currentMenu;
        }
        set
        {
            currentMenu = value;
            ChangeMenu();
        }
    }
    protected virtual void ChangeMenu()
    {
        scoreboardContent.DestroyAllScoreInfos();
        if (CurrentMenu == 0)
        {
            DisplayThisRaceScoreMenu();
        }
        else if (CurrentMenu == 1)
        {
            DisplayAllRaceScoreMenu();
        }
        else if (CurrentMenu == 2)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().LoadNextMap();
            scoreboardContent.content.SetActive(false);
        }
    }

    protected virtual void DisplayThisRaceScoreMenu()
    {
        //Debug.Log(racers == null);
        for (int i = 0; i < racers.Length; i++)
        {
            int place = racers[i].finishedDatas[racers[i].finishedDatas.Count - 1].finishedPlace;
            int score = racers[i].finishedDatas[racers[i].finishedDatas.Count - 1].score;

            scoreboardContent.AddPlaceScoreTimeToDisplay(place, racers[i].name, score, racers[i].finishedDatas[racers[i].finishedDatas.Count - 1].finsihedTime);
        }
    }

    protected virtual void DisplayAllRaceScoreMenu()
    {
        //Bubble sort
        RacerId[] racersInTotalScoreOrder = racers;
        //Debug.Log(racers == null);
        for (int i = 0; i < racers.Length; i++)
        {
            for (int j = 0; j < racers.Length - 1; j++)
            {
                if (racersInTotalScoreOrder[j].GetTotalScore() < racersInTotalScoreOrder[j + 1].GetTotalScore())
                {
                    RacerId temp = racersInTotalScoreOrder[j + 1];
                    racersInTotalScoreOrder[j + 1] = racersInTotalScoreOrder[j];
                    racersInTotalScoreOrder[j] = temp;
                }
            }
        }

        for (int i = 0; i < racersInTotalScoreOrder.Length; i++)
        {

            scoreboardContent.AddPlaceScoreToDisplay(i + 1, racersInTotalScoreOrder[i].name, racersInTotalScoreOrder[i].GetTotalScore());
        }
    }

    public virtual void Next() {
        CurrentMenu = CurrentMenu + 1;
    }

}
