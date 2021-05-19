using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float number;
    public Vector3 respawnPoint = new Vector3(0, 0, 0);
    public float despawnLevel = -40;

    public float GlobalDespawnLevel
    {
        get
        {
            return despawnLevel + transform.position.y;
        }
    }

    public bool despawnLevelGizmo = false;

    public Queue<Kart> respawnQueue;

    public float clearForRespawnDistance = 1f;

    public float despawnDelay = 1f;
    private float lastSpawnedTime = 0f;
    private void Awake()
    {
        respawnQueue = new Queue<Kart>();
    }

    public EmptyTransform RespawnPoint()
    {
        return new EmptyTransform(transform.position + respawnPoint, transform.rotation);
    }

    public void Respawn(Kart kart)
    {
        respawnQueue.Enqueue(kart);
    }

    private Kart justSpawnedKart = null;
    private void SpawnFromQueue()
    {
        Kart kart = respawnQueue.Dequeue();
        kart.transform.position = RespawnPoint().position;
        kart.transform.rotation = RespawnPoint().rotation;
        kart.kartRB.velocity = Vector3.zero;
        kart.kartRB.angularVelocity = Vector3.zero;
        kart.kartRB.isKinematic = true;
        justSpawnedKart = kart;
        kart.gameObject.GetComponent<CheckpointAchiver>().HasRespawned();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<CheckpointAchiver>() != null)
        {
            other.gameObject.GetComponentInParent<CheckpointAchiver>().Achive(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + respawnPoint, .15f);

        if (despawnLevelGizmo)
        {
            Gizmos.color = new Color(255, 0, 0, .35f);
            Gizmos.DrawCube(transform.position + Vector3.up * despawnLevel, new Vector3(500, .01f, 500));

        }

    }

    private void FixedUpdate()
    {
        if(justSpawnedKart != null)
        {
            justSpawnedKart.kartRB.isKinematic = false;
            justSpawnedKart = null;
        }
        
        if(respawnQueue.Count > 0)
        {

            if(Time.time > lastSpawnedTime + despawnDelay)
            {
                SpawnFromQueue();
                lastSpawnedTime = Time.time;
            }

            //OLD VERSION
            /*if(Physics.OverlapSphere(RespawnPoint().position, clearForRespawnDistance).Length < 2) // less then two because it counts itself as well
            {
                SpawnFromQueue();
            }

            *//*                              More Robust but slower version of the above. Concider using if the above stops working
            bool allClear = true;
            Collider[] colliders = Physics.OverlapSphere(RespawnPoint().position, clearForRespawnDistance);
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].gameObject.GetComponent<Rigidbody>() != null)
                {
                    allClear = false;
                }
            }

            if (allClear)
            {
                SpawnFromQueue();
            }*/
        }
        
    }
}
