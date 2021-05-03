using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardContentManager : MonoBehaviour
{
    public Scoreboard scoreboard;

    public GameObject scoreInfoPrefab;

    public GameObject content;
    public GameObject listOfScoresContent;

    List<GameObject> scoreInfos = new List<GameObject>();

    private void Start()
    {
        content.SetActive(false);
    }
    public void AddPlaceScoreTimeToDisplay(int place, string playerName, int score, float time)
    {
        scoreInfos.Add(Instantiate(scoreInfoPrefab,listOfScoresContent.transform));
        scoreInfos[scoreInfos.Count - 1].GetComponent<RectTransform>().anchorMin = new Vector2(0,(8f - place)/8f);
        scoreInfos[scoreInfos.Count - 1].GetComponent<RectTransform>().anchorMax = new Vector2(1, (9f - place) / 8f);
        ScoreInfoWriter scoreInfoWriter = scoreInfos[scoreInfos.Count - 1].GetComponent<ScoreInfoWriter>();

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeString;
        if (timeSpan.Milliseconds.ToString().Length > 1)
        {
            timeString = string.Format("{0}:{1:D2}.{2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds.ToString().Substring(0, 2));

        }
        else
        {
            timeString = string.Format("{0}:{1:D2}.{2:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds.ToString());

        }

        scoreInfoWriter.WriteScoreInfo(place.ToString(), playerName, timeString , "+" + score.ToString());
    }

    public void AddPlaceScoreToDisplay(int place, string playerName, int score)
    {

        scoreInfos.Add(Instantiate(scoreInfoPrefab, listOfScoresContent.transform));
        scoreInfos[scoreInfos.Count - 1].GetComponent<RectTransform>().anchorMin = new Vector2(0, (8f - place) / 8f);
        scoreInfos[scoreInfos.Count - 1].GetComponent<RectTransform>().anchorMax = new Vector2(1, (9f - place) / 8f);
        ScoreInfoWriter scoreInfoWriter = scoreInfos[scoreInfos.Count - 1].GetComponent<ScoreInfoWriter>();

        scoreInfoWriter.WriteScoreInfo(place.ToString(), playerName, "" , score.ToString());


    }

    public void DestroyAllScoreInfos()
    {
        int count = scoreInfos.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(scoreInfos[0]);
            scoreInfos.RemoveAt(0);
        }
    }

    public void Next()
    {
        scoreboard.Next();
    }

}
