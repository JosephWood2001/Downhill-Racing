using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public int checkpointRequiredToFinish = -1;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Kart>() != null)
        {
            if (other.GetComponent<CheckpointAchiver>().currentCheckPoint.number >= checkpointRequiredToFinish || checkpointRequiredToFinish == -1)
            {
                GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>().RacerFinished(other.GetComponent<Kart>().gameObject);

            }
            else if(other.GetComponent<CheckpointAchiver>().canAchive)
            {
                other.GetComponent<CheckpointAchiver>().Respawn();
            }
        }
    }

}
