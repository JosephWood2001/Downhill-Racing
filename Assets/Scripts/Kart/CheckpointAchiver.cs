using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Responsible for remembering the last pasted checkpoint, and detecting when the finish line is passed
 * 
 */
public class CheckpointAchiver : MonoBehaviour
{
    public bool isOrderNessisary;
    public float despawnTime = 3f;
    private float leftGroundTime;

    public float respawnVelocityBoost = 4f;
    private bool boostActive = false; // is true when the kart is waiting to touch the ground to apply boost 
    private float boostDelay = .05f;
    private float hitGroundTime;
    private bool waitingForDelay = false;


    [HideInInspector]
    public bool canAchive = true; // this is set to false when it is put into a respawn queue, the respawn queue will set it back to true when it leaves the queue

    public Checkpoint currentCheckPoint;
    private Rigidbody carRB;
    public LayerMask safeZoneLayerMask;
    public LayerMask outOfBounds;

    private Kart kart;

    private void Start()
    {
        kart = GetComponent<Kart>();

        

        carRB = GetComponent<Rigidbody>();

        canAchive = false;

    }

    public void RaceHasBegan()
    {
        if (currentCheckPoint == null)
        {
            if (GameObject.FindGameObjectWithTag("Start") != null)
            {

                currentCheckPoint = GameObject.FindGameObjectWithTag("Start").GetComponent<Checkpoint>();

            }
            else
            {
                Debug.LogError("No Start Checkpoint in Scene");
                Debug.Break();
            }
        }

        canAchive = true;
    }

    public void FixedUpdate()
    {
        if(currentCheckPoint != null)
        {
            if(transform.position.y < currentCheckPoint.GlobalDespawnLevel)
            {
                Respawn();
            }
            else if (!kart.isGroundedToTrack)
            {
                if (Time.time > leftGroundTime + despawnTime && canAchive)
                {
                    Respawn();
                }
            }
            else
            {
                leftGroundTime = Time.time;
            }

            if (waitingForDelay)
            {
                if (Time.time > hitGroundTime + boostDelay)
                {
                    waitingForDelay = false;
                    //carRB.AddForce(gameObject.transform.forward * respawnVelocityBoost, ForceMode.VelocityChange);  <<-- Currently broken
                }
            }
        }
        


    }

    public void Achive(Checkpoint checkpoint)
    {
        if (canAchive)
        {
            if (isOrderNessisary)
            {
                if (checkpoint.number == currentCheckPoint.number + 1)
                {
                    currentCheckPoint = checkpoint;
                }
                else if (checkpoint.number > currentCheckPoint.number)
                {
                    Respawn();
                }
            }
            else
            {
                currentCheckPoint = checkpoint;
            }
        }
        
        
    }

    public void Respawn()
    {
        canAchive = false;
        leftGroundTime = Time.time;
        currentCheckPoint.Respawn(gameObject.GetComponent<Kart>());
        
    }

    // Called by other classes to tell this class to reset values the should be set when respawned
    public void HasRespawned()
    {
        canAchive = true;
        boostActive = true;
    }

}
