using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSelect : MonoBehaviour
{

    public GameObject[] karts;

    public EmptyTransform previewLocation;

    private int kartInPreview = 0;

    public float previewKartSpeed;
    public float previewKartHandling;
    public float previewKartWeight;

    private GameObject kartObjectInPreview;

    private void OnEnable()
    {
        karts = GameObject.FindGameObjectWithTag("KartCatolog").GetComponent<KartCatolog>().karts;
        DisplayPreviewKart();

    }

    public GameObject SelectKart(int id)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SetPlayer(karts[id]);
        Destroy(kartObjectInPreview);
        return karts[id];
    }

    public void PreviewNextKart()
    {
        kartInPreview++;
        if(kartInPreview >= karts.Length)
        {
            kartInPreview = 0;
        }
        DisplayPreviewKart();
    }

    public void PreviewPreviousKart()
    {
        kartInPreview--;
        if (kartInPreview < 0)
        {
            kartInPreview = karts.Length - 1;
        }
        DisplayPreviewKart();
    }
    
    public void SelectPreviewKart()
    {
        SelectKart(kartInPreview);
    }

    private void DisplayPreviewKart()
    {
        if(kartObjectInPreview != null)
        {
            Destroy(kartObjectInPreview);
        }
        kartObjectInPreview = Instantiate(karts[kartInPreview], previewLocation.position, previewLocation.rotation);
        previewKartHandling = kartObjectInPreview.GetComponent<Kart>().handeling;
        previewKartSpeed = kartObjectInPreview.GetComponent<Kart>().maxSpeed;
        previewKartWeight = kartObjectInPreview.GetComponent<Rigidbody>().mass;
        kartObjectInPreview.GetComponent<CheckpointAchiver>().enabled = false;
        kartObjectInPreview.GetComponent<Kart>().enabled = false;
        kartObjectInPreview.GetComponent<Rigidbody>().useGravity = false;
        kartObjectInPreview.GetComponent<Rigidbody>().isKinematic = false;



    }

    public void RemoveKart()
    {
        Destroy(kartObjectInPreview);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(previewLocation.position, .1f);
        Gizmos.DrawLine(previewLocation.position, previewLocation.position + previewLocation.rotation * Vector3.forward);

    }
}
