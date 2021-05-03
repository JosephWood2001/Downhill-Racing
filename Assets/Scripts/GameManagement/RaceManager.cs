using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public int numberOfRacers = 8; //Temperary

    public RacerId[] racers;
    [HideInInspector]
    public float handycapIntensity = .005f;
    private bool raceActive = false;

    private StartLine startLine;
    private Path path;

    float leaderDistance;

    private float raceStartTime;

    public int raceID = 0;

    public GameObject startRaceAnimation;

    private void FixedUpdate()
    {
        if (raceActive)
        {
            RaceUpdate();
        }

        //for debugging, remove eventually
        if (Input.GetKeyDown(KeyCode.C))
        {
            DebugFinishRace();
        }
    }
    //completes the race for faster debugging
    private void DebugFinishRace()
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (!IsRacerFinished(racers[i]))
            {
                RacerFinished(racers[i].kartObject);
            }
        }

        Destroy(instantiatedStartRaceAnimation);
    }

    private void RaceUpdate()
    {
        UpdateDistances();
        UpdatePos();
        UpdateHandycap();
        
    }

    private void UpdateDistances()
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (!IsRacerFinished(racers[i]))
            {
                racers[i].distanceAlongTrack = path.DistanceAtClostedPoint(racers[i].kartObject.transform.position);
                if (leaderDistance < racers[i].distanceAlongTrack)
                {
                    leaderDistance = racers[i].distanceAlongTrack;
                }
            }
                    
        }
    }

    private void UpdateHandycap()
    {
        for (int i = 0; i < racers.Length; i++)
        {
            if (!IsRacerFinished(racers[i]))
            {
                if (leaderDistance <= racers[i].distanceAlongTrack)
                {
                    racers[i].kart.Handycap = 1;
                }
                else
                {
                    racers[i].kart.Handycap = 1 + ((leaderDistance - racers[i].distanceAlongTrack) * handycapIntensity);
                }
            }
        }

    }

    
    private void UpdatePos()
    {
        int emergencyExit = 1001;
        bool dirty = true;
        while (dirty)
        {
            emergencyExit--;
            dirty = false;
            for (int i = 0; i < racers.Length - 1; i++)
            {
                if (!IsRacerFinished(racers[i]))
                {
                    if (racers[i].distanceAlongTrack < racers[i + 1].distanceAlongTrack)
                    {
                        RacerId temp = racers[i];
                        racers[i] = racers[i + 1];
                        racers[i].Place = i + 1;
                        racers[i + 1] = temp;
                        racers[i + 1].Place = i + 2;
                        dirty = true;
                    }
                }
                
            }
            if(emergencyExit <= 0)
            {
                Debug.Log("EXECUTING EMERGENCY EXIT IN UPDATE POSIOTIONS");
                break;
            }
        }
        
    }

    //Spawns Karts and begins the race
    private GameObject instantiatedStartRaceAnimation;
    public void InitializeRace(RacerId[] racers)
    {
        startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<StartLine>(); // THIS MAY NEED TO BE MOVED TO AN EARLIER RAN PART OF THE CODE, BUT PROBABLY NOT ON START ( the startline might not exist yet )
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>(); // THIS MAY NEED TO BE MOVED TO AN EARLIER RAN PART OF THE CODE, BUT PROBABLY NOT ON START ( the startline might not exist yet )

        raceID++;

        this.racers = racers;
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i].SpawnKart(startLine.GetStartPos(i));
            racers[i].Place = i + 1;
            racers[i].kart.ParkingBreak = true;
        }



        //Calls the animation to play, witch will call beginRace once it has reached GO!
        instantiatedStartRaceAnimation = Instantiate(startRaceAnimation, GameObject.FindGameObjectWithTag("PlayerHud").transform);

    }

    public void BeginRace()
    {
        raceStartTime = Time.time;
        raceActive = true;
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i].kart.ParkingBreak = false;
            racers[i].kart.checkpointAchiver.RaceHasBegan();
        }
    }

    public void RacerFinished(GameObject kart)
    {
        //makes the kart that just finished place the next open place from unfinished players (this is done incase a player with a worse place ends up finishing before a player in a better place)
        int j = 0;
        while (IsRacerFinished(racers[j]))
        {
            j++;
        }
        if (racers[j].kartObject != kart)
        {
            for (int i = j + 1; i < racers.Length; i++)
            {
                if (racers[i].kartObject == kart)
                {
                    RacerId temp = racers[i];
                    racers[i] = racers[j];
                    racers[i].Place = i + 1;
                    racers[j] = temp;
                    racers[j].Place = j + 1;
                }
            }
        }


        float finishTime = Time.time;
        for (int i = 0; i < racers.Length; i++)
        {
            if(racers[i].kartObject == kart)
            {
                int score = Mathf.RoundToInt(Mathf.Pow(20f - (float)(racers[i].Place - 1) * 20f / ((float)racers.Length), 2) / 20f);

                racers[i].finishedDatas.Add(new FinishedData(racers[i].Place,finishTime - raceStartTime, raceID, score));
                if (Camera.main.GetComponent<CameraController>().ObjectToFollow == kart.transform)
                {
                    if (racers[i].Place < racers.Length)
                    {
                        Camera.main.GetComponent<CameraController>().ObjectToFollow = racers[Random.Range(racers[i].Place, racers.Length)].kartObject.transform;
                    }
                }
                racers[i].DestroyKart();
                break;
            }
        }
        bool trueUntilFalse = true;
        for (int i = 0; i < racers.Length; i++)
        {
            if (!IsRacerFinished(racers[i]))
            {
                trueUntilFalse = false;
                break;
            }
        }
        if (trueUntilFalse)
        {
            FinishRace();

        }
        

    }

    public void FinishRace()
    {
        RacerId[] temp = new RacerId[numberOfRacers];
        float[] temp2 = new float[numberOfRacers];
        Debug.Log("Results");
        // ADD THE THREE COMMENTED LINES BELOW FOR TRAINING
        for (int i = 0; i < racers.Length; i++)
        {
            //temp[i] = racers[leaderboard[i]];
            //temp2[i] = finishedTimes[leaderboard[i]];
            Debug.Log("Name: " + racers[i].name + "   Place: " + racers[i].Place);
        }
        //GameObject.FindGameObjectWithTag("AILearning").GetComponent<AILearning>().RaceComplete(temp,temp2);

        raceActive = false;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().RaceComplete();
    }

    public bool IsRacerFinished(RacerId racer)
    {
        if (racer.finishedDatas.Count != 0)
        {
            if (racer.finishedDatas[racer.finishedDatas.Count - 1].raceID == raceID)
            {
                return true;
            }
        }

        return false;
    }

}
