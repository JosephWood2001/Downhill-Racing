using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckpointAchiver))]
public class Kart : MonoBehaviour
{

    BoxCollider boxCollider;


    [Header("Stats")]
    public float acceleration = 1;
    public float maxSpeed = 80f;
    public float steering = 1;
    //Handeling is steering with handycap accounted for (TODO: implement)
    public float Handeling
    {
        get
        {
            return steering;
        }
        set
        {
            steering = value;
        }
    }
    public float grip = 1;
    public float gripFadeSpeed = 100f;

    [Header("Suspension Stats")]
    public float suspensionHeight = .2f;
    public float suspensionStrength = 1f;
    public float suspensionDamping = 1f;

    [Header("Lesser Stats")]
    public Vector3 centerOfMassPos = new Vector3(0, 0, 0);
    public float downwardForce = 1f;
    public float driftGrip = .2f;
    public float gripFadeingLength = 20f;
    public float maxSpeedFadeLength = 20f;
    public float angularSpeed = 1f;

    [Header("Wheel Mesh Refrences")]
    public Transform[] turnMeshes;

    private int place = 1;

    public int Place{
        get
        {
            return place;
        }
        set
        {
            place = value;
        }
    }


    [HideInInspector]
    public Rigidbody kartRB;
    [HideInInspector]
    public CheckpointAchiver checkpointAchiver;
    [HideInInspector]
    public bool isGrounded;

    private KartInput kartInput;

    private bool parkingBreak = true;
    public bool ParkingBreak
    {
        get
        {
            return parkingBreak;
        }

        set
        {
            parkingBreak = value;
        }
    }

    [SerializeField]
    private float handycap = 1f;

    public float Handycap
    {
        get
        {
            return handycap;
        }
        set
        {
            handycap = value;
            ApplyHandyCap();
        }
    }

    private void Start()
    {
        kartRB = gameObject.GetComponent<Rigidbody>();
        kartRB.centerOfMass = centerOfMassPos;

        boxCollider = GetComponent<BoxCollider>();

        ApplyHandyCap();

        checkpointAchiver = gameObject.GetComponent<CheckpointAchiver>();

        if(kartInput == null)
        {
            kartInput = new KartInput();
            parkingBreak = false;
        }
    }

    
    protected virtual void Update()
    {

        UpdateTurnMeshes();


        if (kartInput.Reset == true && checkpointAchiver.canAchive)
        {
            checkpointAchiver.Respawn();
        }

    }

    float compression;
    Vector3 impactPoint;
    Vector3 impactNormal;
    private void FixedUpdate()
    {
        Vector3 frontRight = transform.position + transform.rotation * (GetComponent<BoxCollider>().center - Vector3.up * GetComponent<BoxCollider>().size.y / 2f + Vector3.right * GetComponent<BoxCollider>().size.x / 2f + Vector3.forward * GetComponent<BoxCollider>().size.z / 2f);
        Vector3 frontLeft = transform.position + transform.rotation * (GetComponent<BoxCollider>().center - Vector3.up * GetComponent<BoxCollider>().size.y / 2f - Vector3.right * GetComponent<BoxCollider>().size.x / 2f + Vector3.forward * GetComponent<BoxCollider>().size.z / 2f);
        Vector3 backRight = transform.position + transform.rotation * (GetComponent<BoxCollider>().center - Vector3.up * GetComponent<BoxCollider>().size.y / 2f + Vector3.right * GetComponent<BoxCollider>().size.x / 2f - Vector3.forward * GetComponent<BoxCollider>().size.z / 2f);
        Vector3 backLeft = transform.position + transform.rotation * (GetComponent<BoxCollider>().center - Vector3.up * GetComponent<BoxCollider>().size.y / 2f - Vector3.right * GetComponent<BoxCollider>().size.x / 2f - Vector3.forward * GetComponent<BoxCollider>().size.z / 2f);

        bool someContact = false;
        if (Suspend(frontRight, out compression, out impactPoint, out impactNormal))
        {
            someContact = true;
        }
        if (Suspend(frontLeft, out compression, out impactPoint, out impactNormal))
        {
            someContact = true;
        }
        if (Suspend(backRight, out compression, out impactPoint, out impactNormal))
        {
            someContact = true;
        }
        if (Suspend(backLeft, out compression, out impactPoint, out impactNormal))
        {
            someContact = true;
        }
        isGrounded = someContact;

        if (someContact)
        {
            Accelerate(GetInput().VerticalInput * acceleration, GetAccellerationDirection(impactNormal));
            Turn(steering * GetInput().HorizontalInput);
            DownwardForce(downwardForce, -impactNormal);
            GripForce(grip, GetAccellerationDirectionRight(impactNormal));
        }


        if (!someContact)
        {
            Vector3 localAngularVelocity = Quaternion.Inverse( transform.rotation ) * kartRB.angularVelocity;
            kartRB.AddRelativeTorque((new Vector3(
                -((localAngularVelocity.x - kartInput.AirVerticalInput * angularSpeed) * .05f),
                -((localAngularVelocity.y - kartInput.AirHorizontalInput * angularSpeed) * .05f),
                -((localAngularVelocity.z + kartInput.AirRollInput * angularSpeed) * .05f)
                )), ForceMode.VelocityChange);
        }
        else
        {

            kartRB.AddTorque(Vector3.zero, ForceMode.Acceleration);
        }
    }


    void GripForce(float grip, Vector3 directionRight)
    {
        float slide = Vector3.Dot(kartRB.velocity, directionRight);
        float gripFadeAmount = Mathf.Clamp(gripFadeSpeed - kartRB.velocity.magnitude, driftGrip * gripFadeingLength, gripFadeingLength) / gripFadeingLength;
        kartRB.AddForce(-slide * grip * gripFadeAmount * directionRight, ForceMode.VelocityChange);

    }

    void DownwardForce(float force, Vector3 direction)
    {
        kartRB.AddForce(force * direction, ForceMode.VelocityChange);

    }

    void Accelerate(float force, Vector3 forward)
    {
        float forceFadeAmount = Mathf.Clamp(maxSpeed - kartRB.velocity.magnitude, 0, maxSpeedFadeLength) / maxSpeedFadeLength;

        kartRB.AddForce(forceFadeAmount * force * forward, ForceMode.VelocityChange);
    }

    void Turn(float turn)
    {
        turn *= VelocityDirection();

        float turnSlide = (kartRB.angularVelocity.y - turn * 2f);

        kartRB.AddTorque(-turnSlide * transform.up, ForceMode.VelocityChange);
    }

    bool Suspend(Vector3 location, out float compression, out Vector3 impactPoint, out Vector3 impactNormal)
    {
        RaycastHit hit;
        if (Physics.Raycast(location, -transform.up, out hit, suspensionHeight))
        {

            compression = (suspensionHeight - hit.distance) / suspensionHeight;
            impactPoint = hit.point;
            impactNormal = hit.normal;

            //adds force from suspension

            kartRB.AddForceAtPosition((suspensionStrength * compression - suspensionDamping * Vector3.Dot(transform.up, kartRB.GetPointVelocity(location))) * transform.up, location, ForceMode.VelocityChange);
            return true;
        }
        else
        {
            compression = 0;
            //sentinal value for didn't hit
            impactPoint = Vector3.negativeInfinity;
            impactNormal = transform.up;
            return false;
        }


    }


    Vector3 GetAccellerationDirection(Vector3 impactNormal)
    {
        return (Quaternion.AngleAxis(90f, Vector3.Cross(impactNormal, gameObject.transform.forward)) * impactNormal).normalized;

    }

    Vector3 GetAccellerationDirectionRight(Vector3 impactNormal)
    {
        return (Quaternion.AngleAxis(90f, Vector3.Cross(impactNormal, gameObject.transform.right)) * impactNormal).normalized;

    }

    float VelocityDirection()
    {
        float direction = Vector3.Dot(transform.forward, kartRB.velocity.normalized) / Mathf.Abs(Vector3.Dot(transform.forward, kartRB.velocity.normalized));
        if (float.IsNaN(direction))
        {
            direction = 1;
        }
        return direction;
    }


    public void SetInput(KartInput kartInput)
    {
        this.kartInput = kartInput;

    }

    public KartInput GetInput()
    {
        return kartInput;

    }

    /**updates all wheels to their respective poses from their wheel colliders
     */
    private void UpdateTurnMeshes()
    {


    }

    public void SetKartInput(KartInput input)
    {
        kartInput = input;
    }

    public void ApplyHandyCap()
    {
        
    }

}
