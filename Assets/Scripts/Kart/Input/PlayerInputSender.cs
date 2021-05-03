using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputSender : InputSender
{
    [Header("InputSettings")]
    public bool instantInput = false;
    public float noneInstantInputInputRate = 1f;
    public float resetDelay = 1.5f;
    private bool delayStarted = false;
    private float delayStartTime;
    private bool resetKeyLifted = true;
    public GameObject resetAnimation;

    private bool lookingForward = true;

    private GameObject resetAnimationInstance;
    protected override void CreateInput()
    {
        base.CreateInput();

        vertical = Input.GetAxisRaw("Vertical");
        airHorizontal = Input.GetAxisRaw("AirHorizontal");
        airVertical = Input.GetAxisRaw("AirVertical");
        airRoll = Input.GetAxisRaw("AirRoll");
        

        if (instantInput)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            breaking = Input.GetAxisRaw("Break");

        }
        else
        {
            if(Input.GetAxisRaw("Horizontal") == 0)
            {
                horizontal = Mathf.Lerp(horizontal, Input.GetAxisRaw("Horizontal"), noneInstantInputInputRate * Time.deltaTime * 5f);
            }
            else
            {

                horizontal = Mathf.Lerp(horizontal, Input.GetAxisRaw("Horizontal"), noneInstantInputInputRate * Time.deltaTime);
            }

            
            if (Input.GetAxisRaw("Break") == 0f)
            {
                breaking = 0f;
            }
            breaking = Mathf.Lerp(breaking, Input.GetAxisRaw("Break"), noneInstantInputInputRate * Time.deltaTime);
        }

        if(Input.GetAxis("Reset") > 0 && !delayStarted && resetKeyLifted && !kart.ParkingBreak)
        {
            delayStarted = true;
            resetKeyLifted = false;
            delayStartTime = Time.time;

            if(resetAnimation != null)
            {
                resetAnimationInstance = Instantiate(resetAnimation, GameObject.FindGameObjectWithTag("PlayerHud").transform);
            }
            
        }
        if(Input.GetAxis("Reset") == 0)
        {
            resetKeyLifted = true;
            delayStarted = false;

            if(resetAnimationInstance != null)
            {
                Destroy(resetAnimationInstance);
            }
        }
        if (delayStarted && Time.time > delayStartTime + resetDelay)
        {
            reset = true;
            delayStarted = false;

            if (resetAnimationInstance != null)
            {
                Destroy(resetAnimationInstance);
            }
        }

        if (Input.GetAxis("LookBack") > 0 && lookingForward)
        {
            lookingForward = false;
            if (Camera.main.GetComponent<CameraController>() != null)
            {
                Camera.main.GetComponent<CameraController>().LookingForward = false;
            }
        }
        if(Input.GetAxis("LookBack") == 0 && !lookingForward)
        {
            lookingForward = true;
            if (Camera.main.GetComponent<CameraController>() != null)
            {
                Camera.main.GetComponent<CameraController>().LookingForward = true;
            }
        }
    }
}
