using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsManager : MonoBehaviour
{
    RacerId[] racersInTotalScoreOrder;

    public Transform first;
    public Transform second;
    public Transform third;

    public void PlayResults(RacerId[] racers)
    {
        //Bubble sort
        racersInTotalScoreOrder = racers;
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
        Camera.main.GetComponent<CameraController>().ObjectToFollow = InstantiateDisplayKart(racers[0], first).transform;
        InstantiateDisplayKart(racers[1], second);
        InstantiateDisplayKart(racers[2], third);
    }



    private GameObject InstantiateDisplayKart(RacerId racer,Transform transform)
    {
        GameObject kartObject = Instantiate(racer.kartObjectPrefab, transform);
        kartObject.GetComponent<CheckpointAchiver>().enabled = false;
        kartObject.GetComponent<Kart>().enabled = false;
        kartObject.GetComponent<Rigidbody>().isKinematic = true;
        return kartObject;
    }
}
