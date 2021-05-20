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
    public float acceleration = .1f;
    public float TrueAcceleration
    {
        get
        {
            return acceleration * Mathf.Pow(Handycap, handycapOnAcceleration);
        }
    }
    public float maxSpeed = 20f;
    public float TrueMaxSpeed
    {
        get
        {
            return maxSpeed * Mathf.Pow(Handycap, handycapOnMaxSpeed);
        }
    }
    public float steering = 1;
    public float TrueSteering
    {
        get
        {
            return steering * Mathf.Pow(Handycap,handycapOnSteering);
        }
    }
    public float grip = .5f;
    public float gripFadeSpeed = 20f;

    [Header("Suspension Stats")]
    public float suspensionHeight = .25f;
    public float suspensionStrength = 1f;
    public float suspensionDamping = .1f;

    [Header("Lesser Stats")]
    public Vector3 centerOfMassPos = new Vector3(0, 0.1f, -0.24f);
    public float breakingForce = .3f;
    public float downwardForce = 0.2f;
    public float driftGrip = 0.05f;
    public float gripFadeingLength = 8f;
    public float maxSpeedFadeLength = 5f;
    public float angularSpeed = 1f;
    public float handycapOnSteering = 0.5f;
    public float handycapOnMaxSpeed = 0.5f;
    public float handycapOnAcceleration = 1f;
    public float maxSteeringAngle = 30f;

    [Header("Mesh Refrences")]
    public Transform[] turnMeshes;
    public Transform[] wheelMeshes;

    [Header("View Only")]
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
        }
    }

    private void Start()
    {
        kartRB = gameObject.GetComponent<Rigidbody>();
        kartRB.centerOfMass = centerOfMassPos;

        boxCollider = GetComponent<BoxCollider>();

        checkpointAchiver = gameObject.GetComponent<CheckpointAchiver>();

        if(kartInput == null)
        {
            kartInput = new KartInput();
            parkingBreak = false;
        }
    }

    
    protected virtual void Update()
    {
        

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
        Debug.Log(Handycap);

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
            Accelerate(GetInput().VerticalInput * TrueAcceleration, GetAccellerationDirection(impactNormal));
            Turn(TrueSteering * GetInput().HorizontalInput);
            DownwardForce(downwardForce, -impactNormal);
            GripForce(grip, GetAccellerationDirectionRight(impactNormal));
            BreakForce(breakingForce * kartInput.BreakingInput, GetAccellerationDirection(impactNormal));
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
        float forceFadeAmount = Mathf.Clamp(TrueMaxSpeed - kartRB.velocity.magnitude, 0, maxSpeedFadeLength) / maxSpeedFadeLength;

        //significently less acceleration in reverse, and none when moving forward
        if (force < 0f && VelocityDirection() == 1 && kartRB.velocity.magnitude > 2f)
        {
            forceFadeAmount = 0;
        }
        else if(force < 0)
        {
            forceFadeAmount *= .5f;
        }

        kartRB.AddForce(forceFadeAmount * force * forward, ForceMode.VelocityChange);
    }

    void BreakForce(float force, Vector3 forward)
    {
        kartRB.AddForce(-VelocityDirection() * force * forward, ForceMode.VelocityChange);

    }

    void Turn(float turn)
    {
        turn *= VelocityDirection();
        //no turn if kart isn't moving
        if (kartRB.velocity.magnitude < 1f)
        {
            turn *= kartRB.velocity.magnitude;
        }

        float turnSlide = (kartRB.angularVelocity.y - turn * 2f);
        //no turn if kart isn't moving
        if(kartRB.velocity.magnitude < .01f)
        {
            turn = 0;
        }
        kartRB.AddTorque(-turnSlide * transform.up, ForceMode.VelocityChange);

        UpdateTurnMeshes(turnMeshes, maxSteeringAngle * kartInput.HorizontalInput);
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

    private void UpdateTurnMeshes(Transform[] meshes, float steeringAngle)
    {
        foreach(Transform mesh in meshes)
        {
            mesh.localRotation = Quaternion.Euler(0, steeringAngle, 0);
        }

    }

    private void UpdateWheelMeshes()
    {
        //TODO

    }

    public void SetKartInput(KartInput input)
    {
        kartInput = input;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMassPos, .05f);
    }

}
