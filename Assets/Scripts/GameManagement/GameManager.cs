using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [DisplayWithoutEdit]
    public string loadedCourse = "";

    public virtual void RaceComplete()
    {

    }

    public virtual bool LoadNextMap() { return false; }

    public virtual void SeasonComplete() { }

    public virtual void SetPlayer(GameObject kart) { }
}
