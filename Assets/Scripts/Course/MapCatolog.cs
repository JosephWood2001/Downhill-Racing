using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCatolog : MonoBehaviour
{
    public Cup[] cups;

    [Serializable]
    public class Cup
    {
        public GameObject cupMiniIcon;
        [Header("Must be the same length")]
        public String[] courses;
        public GameObject[] coursesMiniIcons;
    }
}
