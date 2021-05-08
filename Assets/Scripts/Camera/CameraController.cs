using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform objectToFollow;

    public Transform ObjectToFollow
    {
        get
        {
            return objectToFollow;
        }
        set
        {
            objectToFollow = value;
            targetRB = objectToFollow.GetComponent<Rigidbody>();
            currentForward = objectToFollow.forward;
        }
    }
    private Rigidbody targetRB;

    public float minDistance = 3f;
    public float maxDistance = 5f;
    public float speedEffectivenessOnDistance = 1f;

    public float cameraAngle = 10f;
    public float cameraHeight = 1f;

    private Vector3 kartForward;//the normalized vector for the direction the camera should follow (without any Euler Z degrees)
    public float speedForForwardVectorOnlyDependsOnVelocity = 1f;

    public float rotationDampen = 1f;
    public float forwardDampen = 1f;
    private Vector3 currentForward;

    private bool lookingForward = true;
    public bool LookingForward
    {
        get
        {
            return lookingForward;
        }
        set
        {
            if(lookingForward != value)
            {
                lookingForward = value;
            }
            
        }
    }

    private void Start()
    {
        if(objectToFollow != null)
        {
            ObjectToFollow = objectToFollow;
        }
        
    }

    public void LookAtTarget()
    {
        Vector3 lookDirection = (objectToFollow.position + Vector3.up * cameraHeight) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f - Mathf.Pow(.5f, Time.deltaTime * rotationDampen));

    }

    private Vector3 kartUp;
    public void MoveToTarget()
    {
        kartForward = Vector3.Lerp(objectToFollow.forward, targetRB.velocity.normalized, Mathf.Clamp( targetRB.velocity.magnitude/speedForForwardVectorOnlyDependsOnVelocity,0f,1f )).normalized;
        currentForward = Vector3.Lerp(currentForward, kartForward, 1f - Mathf.Pow(.5f, Time.deltaTime * forwardDampen));
        kartUp = Quaternion.AngleAxis(90f, Vector3.Cross(currentForward, Vector3.up)) * currentForward;

        float distance = DampenFunction(minDistance, maxDistance, speedEffectivenessOnDistance, targetRB.velocity.magnitude);
        
        transform.position = objectToFollow.position - currentForward * distance + kartUp * cameraHeight;


    }

    private static float DampenFunction(float min, float max, float multiplier, float x)
    {
        return -(max - min) / (multiplier * x + 1f) + max;
    }

    private void Update()
    {
        if(ObjectToFollow != null)
        {
            
            MoveToTarget();
            LookAtTarget();
        }
        
        
    }
}
