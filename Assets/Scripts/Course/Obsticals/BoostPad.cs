using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public float boostAmount = 2f;

    List<Rigidbody> inZone = new List<Rigidbody>();

    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRigidbody;
        if (other.GetComponent<Rigidbody>() != null)
        {
            otherRigidbody = other.GetComponent<Rigidbody>();
        }
        else if(other.gameObject.GetComponentInParent<Rigidbody>() != null)
        {
            otherRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();
        }
        else
        {
            return;
        }

        foreach(Rigidbody rigid in inZone)
        {
            if(rigid == otherRigidbody)
            {
                inZone.Add(rigid);
                return;
            }
        }
        otherRigidbody.AddForce(transform.forward * boostAmount, ForceMode.VelocityChange);
        inZone.Add(otherRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody otherRigidbody;
        if (other.GetComponent<Rigidbody>() != null)
        {
            otherRigidbody = other.GetComponent<Rigidbody>();
        }
        else if (other.gameObject.GetComponentInParent<Rigidbody>() != null)
        {
            otherRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();
        }
        else
        {
            return;
        }
        inZone.Remove(otherRigidbody);
        
    }
}
