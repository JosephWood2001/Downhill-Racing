using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InputSender : MonoBehaviour
{
    //input refrence
    protected KartInput kartInput;
    //kart refrence
    protected Kart kart;

    protected float horizontal = 0f;
    protected float vertical = 0f;
    protected float breaking = 0f;
    protected bool reset = false;

    protected float airHorizontal = 0f;
    protected float airVertical = 0f;
    protected float airRoll = 0f;

    protected bool isReady = false;


    protected virtual void Update()
    {
        if (kartInput == null)
        {
            if (GameObject.Find("Kart").GetComponent<Kart>().GetInput() != null)
            {
                kartInput = GameObject.Find("Kart").GetComponent<Kart>().GetInput();
                kart = GameObject.Find("Kart").GetComponent<Kart>();
            }
            else
            {
                Debug.Log("Couldn't Find Kart");
            }

        }

        CreateInput();
        if(kartInput != null)
        {
            SendInputs();
        }
        
    }

    protected virtual void SendInputs()
    {
        kartInput.HorizontalInput = horizontal;
        kartInput.VerticalInput = vertical;
        kartInput.BreakingInput = breaking;

        kartInput.AirHorizontalInput = airHorizontal;
        kartInput.AirVerticalInput = airVertical;
        kartInput.AirRollInput = airRoll;

        if (reset) //Makes the input a one time impulse
        {
            reset = false;
            kartInput.Reset = true;
        }
    }

    protected virtual void CreateInput()
    {
        
    }

    public void SetKartInputAndKart(KartInput input, Kart kart)
    {
        kartInput = input;
        this.kart = kart;
    }

}
