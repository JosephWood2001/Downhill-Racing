using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStartAnimation : MonoBehaviour
{
    public void CallRaceManagerToBeginRace()
    {
        GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>().BeginRace();
    }

    public void DestroyMyself()
    {
        Destroy(gameObject);
    }
}
