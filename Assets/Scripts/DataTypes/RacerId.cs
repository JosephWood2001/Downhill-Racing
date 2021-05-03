using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RacerId
{
    public string name;
    public GameObject kartObjectPrefab;
    public GameObject kartObject;
    public Kart kart;
    public KartInput input;
    public GameObject inputSenderPrefab;
    public GameObject inputSender;
    public bool attachCamera;
    private PlayerHud playerHud;

    private int place = 1;
    public float distanceAlongTrack = 0;

    public List<FinishedData> finishedDatas = new List<FinishedData>();

    public RacerId()
    {
        
    }

    public RacerId(string name, GameObject kart, GameObject inputSender)
    {
        this.name = name;
        this.kartObjectPrefab = kart;
        this.input = new KartInput();
        this.inputSenderPrefab = inputSender;
        this.attachCamera = false;

    }

    public RacerId(string name, GameObject kart, GameObject inputSender, bool attachCamera)
    {
        this.name =  name;
        this.kartObjectPrefab = kart;
        this.input = new KartInput();
        this.inputSenderPrefab = inputSender;
        this.attachCamera = attachCamera;
        
        
    }

    public void SpawnKart(EmptyTransform emptyTransform)
    {
        this.input = new KartInput();

        inputSender = GameObject.Instantiate(inputSenderPrefab);
        

        kartObject = GameObject.Instantiate(kartObjectPrefab, emptyTransform.position, emptyTransform.rotation);
        kart = kartObject.GetComponent<Kart>();
        kart.SetKartInput(input);

        this.inputSender.GetComponent<InputSender>().SetKartInputAndKart(input, kart);


        if (attachCamera)
        {
            Camera.main.GetComponent<CameraController>().ObjectToFollow = this.kartObject.transform;
            playerHud = GameObject.FindGameObjectWithTag("PlayerHud").GetComponent<PlayerHud>();
            playerHud.content.SetActive(true);
        }
    }

    public int Place
    {
        get
        {
            return place;
        }
        set
        {
            place = value;
            if (attachCamera)
            {
                playerHud.ChangePlace(place);
            }
        }
    }

    public int GetTotalScore()
    {
        int score = 0;
        foreach(FinishedData finishedData in finishedDatas)
        {
            score += finishedData.score;
        }
        return score;
    }

    public void DestroyKart()
    {
        GameObject.Destroy(inputSender);
        GameObject.Destroy(kartObject);
    }
}
