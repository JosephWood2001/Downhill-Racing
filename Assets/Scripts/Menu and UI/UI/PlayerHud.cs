using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public GameObject place;
    public GameObject content;

    private TextMeshProUGUI placeText;
    private void Start()
    {
        content.SetActive(false);
        placeText = place.GetComponent<TextMeshProUGUI>();
        
    }

    public void ChangePlace(int place)
    {
        placeText.text = "Place: " + place;
    }
}
