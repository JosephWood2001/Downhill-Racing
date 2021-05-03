using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreInfoWriter : MonoBehaviour
{
    public TextMeshProUGUI place;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI time;
    public TextMeshProUGUI score;

    public void WriteScoreInfo(string place, string name, string time, string score)
    {
        this.place.text = place;
        this.playerName.text = name;
        this.time.text = time;
        this.score.text = score;
    }
}
