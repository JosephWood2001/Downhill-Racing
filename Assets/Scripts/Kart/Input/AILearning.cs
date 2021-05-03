using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

public class AILearning : MonoBehaviour
{
    public int numberOfRacers = 8;

    public RacerId[] racers;
    public float[] curveIntensity; //the intensity that a more intensive curve will slowdown the kart
    public float[] speedLimitIntensity;
    public float[] handelingSpeedLimitIntensity;

    public GameObject kart;
    public GameObject inputSender;

    public int raceNumber = 0;

    public float mutationRate = .05f;

    public float gameSpeed = 1f;

    public float finishedTimeEffectivness = 50f;

    public RaceManager raceManager;
    // Start is called before the first frame update
    void Start()
    {
        raceManager = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();

        racers = new RacerId[numberOfRacers];
        curveIntensity = new float[numberOfRacers];
        speedLimitIntensity = new float[numberOfRacers];
        handelingSpeedLimitIntensity = new float[numberOfRacers];
        for (int i = 0; i < numberOfRacers; i++)
        {
            racers[i] = new RacerId("newPlayer" + Random.Range(0, 1000), kart, inputSender);
        }
    }

    public void RaceComplete(RacerId[] racers, float[] finishedTimes)
    {
        float[] finishedIndex = new float[numberOfRacers];
        finishedIndex[0] = Mathf.Pow(finishedTimes[0],finishedTimeEffectivness);
        for (int i = 1; i < finishedIndex.Length; i++)
        {
            finishedIndex[i] = 2f * finishedIndex[0] - Mathf.Pow(finishedTimes[i],finishedTimeEffectivness) + finishedIndex[i - 1];
        }

        float[] oldCurveIntensity = curveIntensity;
        float[] oldSpeedLimitIntensity = speedLimitIntensity;
        float[] oldHandelingSpeedLimitIntensity = handelingSpeedLimitIntensity;

        int[] startPosScrambler = new int[numberOfRacers];

        for (int i = 0; i < startPosScrambler.Length; i++)
        {
            startPosScrambler[i] = -1; //-1 means null
        }

        for (int i = 0; i < startPosScrambler.Length; i++)
        {
            int random = Random.Range(0, numberOfRacers);
            while(startPosScrambler[random] != -1)
            {
                random = Random.Range(0, numberOfRacers);
            }
            startPosScrambler[random] = i;
        }

        raceNumber++;
        SaveData(racers,finishedTimes);
        /*
        for (int i = 0; i < numberOfRacers; i++)
        {
            if(racers[startPosScrambler[0]] == this.racers[i])
            {
                curveIntensity[startPosScrambler[0]] = oldCurveIntensity[i];
                speedLimitIntensity[startPosScrambler[0]] = oldSpeedLimitIntensity[i];
                handelingSpeedLimitIntensity[startPosScrambler[0]] = oldHandelingSpeedLimitIntensity[i];
                break;
            }
        }
        for (int i = 0; i < numberOfRacers; i++)
        {
            if (racers[startPosScrambler[1]] == this.racers[i])
            {
                curveIntensity[startPosScrambler[1]] = oldCurveIntensity[i];
                speedLimitIntensity[startPosScrambler[1]] = oldSpeedLimitIntensity[i];
                handelingSpeedLimitIntensity[startPosScrambler[1]] = oldHandelingSpeedLimitIntensity[i];
                break;
            }
        }
        this.racers[startPosScrambler[0]] = racers[0];
        this.racers[startPosScrambler[1]] = racers[1];
        */
        float tempRand;
        int rand = 0;
        
        for (int i = 0; i < this.racers.Length; i++)
        {
            tempRand = Random.Range(0, finishedIndex[finishedIndex.Length - 1]);
            for (int j = 0; j < finishedIndex.Length; j++)
            {
                
                if (tempRand < finishedIndex[j])
                {
                    rand = j;
                    break;
                }

            }

            this.racers[startPosScrambler[i]] = new RacerId("newPlayer" + Random.Range(0, 1000), kart, inputSender);
            curveIntensity[startPosScrambler[i]] = oldCurveIntensity[rand] * (1 + Random.Range(-mutationRate, mutationRate));
            speedLimitIntensity[startPosScrambler[i]] = oldSpeedLimitIntensity[rand] * (1 + Random.Range(-mutationRate, mutationRate));
            handelingSpeedLimitIntensity[startPosScrambler[i]] = oldHandelingSpeedLimitIntensity[rand] * (1 + Random.Range(-mutationRate, mutationRate));
        }

        StartRace();
    }

    void SaveData(RacerId[] racers, float[] finishedTimes)
    {
        string path = Application.dataPath + "/Log.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "fileCreated");
        }

        string content = "";
        for (int i = 0; i < racers.Length; i++)
        {
            for (int j = 0; j < this.racers.Length; j++)
            {
                if(racers[i] == this.racers[j])
                {
                    content += curveIntensity[j] + " " + speedLimitIntensity[j] + " " + handelingSpeedLimitIntensity[j] + " " + finishedTimes[j] + "\n";
                    continue;
                }
            }
        }

        File.WriteAllText(path,content);
    }

    void StartRace()
    {
        raceManager.InitializeRace(racers);
        for (int i = 0; i < racers.Length; i++)
        {
            
            if(curveIntensity[i] <= 0)
            {
                curveIntensity[i] = racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().curveIntensity;
            }
            if (speedLimitIntensity[i] <= 0)
            {
                speedLimitIntensity[i] = racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().speedLimitIntensity;
            }
            if (handelingSpeedLimitIntensity[i] <= 0)
            {
                handelingSpeedLimitIntensity[i] = racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().handelingSpeedLimitIntensity;
            }
            racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().curveIntensity = curveIntensity[i];
            racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().speedLimitIntensity = speedLimitIntensity[i];
            racers[i].inputSender.gameObject.GetComponent<ComputerInputSender>().handelingSpeedLimitIntensity = handelingSpeedLimitIntensity[i];

        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = gameSpeed;

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //RaceComplete(racers);
            DebugStuff();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartRace();
        }
    }

    void DebugStuff()
    {

    }
}
