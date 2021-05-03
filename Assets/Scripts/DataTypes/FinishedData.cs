using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FinishedData
{
    public int finishedPlace;
    public float finsihedTime;
    public int raceID;
    public int score;

    public FinishedData(int finishedPlace, float finsihedTime, int raceID, int score)
    {
        this.finishedPlace = finishedPlace;
        this.finsihedTime = finsihedTime;
        this.raceID = raceID;
        this.score = score;
    }
}
