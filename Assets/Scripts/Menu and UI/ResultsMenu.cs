using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsMenu : MonoBehaviour
{
    //call season done to the gamemanager to do whatever the race manager needs to do when a season is finished
    public void SeasonDone()
    {

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SeasonComplete();
    }
}
