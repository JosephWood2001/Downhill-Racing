using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckpointAchiver))]
public class Kart : MonoBehaviour
{
    [Header("Wheel Refrences")]
    public Wheel[] wheels;

    [Header("Wheel Mesh Refrences")]
    public Transform[] turnMeshes;

    [Header("Kart Stats")]
    public float runningStartSpeed = 2f;
    public float maxSteerAngle = 15f;
    public float maxSpeed = 5;
    //the rate at whitch torque can be applied (dependant on karts maxSpeed)
    float angularSpeed;
    public float handeling = 5; //effect is squared

    public float Handeling // gives the handeling of the kart, handycap included
    {
        get
        {
            return handeling * handycap;
        }
    }

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


    //Center of mass on the rigid body
    public Vector3 centerOfMassPos = new Vector3(0, 0, 0);

    [HideInInspector]
    public Rigidbody kartRB;
    [HideInInspector]
    public CheckpointAchiver checkpointAchiver;

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
    [SerializeField]
    private float slantAssist = 1f;

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

    public float SlantAssist
    {
        get
        {
            return slantAssist;
        }
        set
        {
            if(slantAssist == value)
            {
                return;
            }
            slantAssist = value;
            ApplyHandyCap();
        }
    }

    private void Start()
    {
        kartRB = gameObject.GetComponent<Rigidbody>();

        kartRB.centerOfMass = centerOfMassPos;

        ApplyHandyCap();//must be after kartRB is set

        checkpointAchiver = gameObject.GetComponent<CheckpointAchiver>();

        angularSpeed = .5f * Mathf.Pow( maxSpeed, .5f);
    }
    
    protected virtual void Update()
    {
        foreach (Wheel wheel in wheels)
        {

            wheel.UpdateWheelPose();
        }

        UpdateTurnMeshes();

        

        if (kartInput.Reset == true && checkpointAchiver.canAchive)
        {
            checkpointAchiver.Respawn();
        }

        if (kartInput.HorizontalInput > 0 && (gameObject.transform.rotation.eulerAngles.z > 0 && gameObject.transform.rotation.eulerAngles.z < 180))
        {
            SlantAssist = 1f / Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * gameObject.transform.rotation.eulerAngles.z));
        }
        else if (kartInput.HorizontalInput < 0 && (gameObject.transform.rotation.eulerAngles.z > 180 && gameObject.transform.rotation.eulerAngles.z < 360))
        {
            SlantAssist = 1f / Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * gameObject.transform.rotation.eulerAngles.z));
        }
        else
        {
            SlantAssist = 1;
        }
    }

    private void FixedUpdate()
    {

        if (!ParkingBreak)
        {
            bool condition1 = kartRB.velocity.magnitude < runningStartSpeed;
            bool condition2 = (Quaternion.Inverse(transform.rotation) * kartRB.velocity).z < .1f;
            bool condition3 = checkpointAchiver.isGrounded;
            bool condition4 = kartInput.VerticalInput < 0;
            if (condition1 && condition2 && condition3 && condition4)
            {
                foreach (Wheel wheel in wheels)
                {
                    wheel.Accelerate(true, kartInput.VerticalInput);
                }
            }
            else
            {
                foreach (Wheel wheel in wheels)
                {
                    wheel.Accelerate(false, kartInput.VerticalInput);
                }
            }

            if (condition1 && condition3 && !condition4)
            {
                kartRB.AddRelativeForce(new Vector3(0,0,2f), ForceMode.Acceleration);
            }
            else
            {
                kartRB.AddRelativeForce(Vector3.zero, ForceMode.Acceleration);
                
            }
        }

        foreach (Wheel wheel in wheels)
        {

            wheel.Steer(kartInput.HorizontalInput);

            if (!ParkingBreak)
            {
                wheel.ApplyBreaks(kartInput.BreakingInput);
            }
            else
            {
                wheel.ApplyBreaks(1f);
            }

        }

        if (!checkpointAchiver.isGrounded)
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

    /**For children of this class to update the inputs controlling this kart
     * also clamps the inputs to their respective ranges
     */
    protected void SetInput(KartInput kartInput)
    {
        this.kartInput = kartInput;

    }

    /**updates all wheels to their respective poses from their wheel colliders
     */
    private void UpdateTurnMeshes()
    {

        foreach (Transform turnMesh in turnMeshes)
        {
            turnMesh.localRotation = Quaternion.Euler(0, kartInput.HorizontalInput * maxSteerAngle, 0);
        }

    }

    public void SetKartInput(KartInput input)
    {
        kartInput = input;
    }

    public void ApplyHandyCap()
    {
        handycap = Mathf.Clamp(handycap, 0f, 2f);
        
        //formula that determins the speed of the kart through changing its drag
        //
        //affected by the speed of the kart so that accelleration is faster and momentum matters less
        kartRB.drag = 1f / maxSpeed / handycap * Mathf.Pow((kartRB.velocity.magnitude / maxSpeed),5);
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].isSteer)
            {

                ApplyHandyCap(wheels[i], handycap * SlantAssist);
            }
        }
    }

    WheelFrictionCurve wheelFrictionCurve;
    private void ApplyHandyCap(Wheel wheel, float handycap)
    {
        wheelFrictionCurve = wheel.GetWheelCollider().sidewaysFriction;
        SetFrictionCurveFromHandling(ref wheelFrictionCurve, handeling * handycap); 
        wheel.SetWheelFrictionCurve(wheelFrictionCurve);
    }

    private void SetFrictionCurveFromHandling(ref WheelFrictionCurve wheelFriction, float handeling)
    {
        SetHandlingFrictionCurve(ref wheelFriction, handeling / 5f);
    }

    private void SetHandlingFrictionCurve(ref WheelFrictionCurve wheelFriction, float stiffness)
    {
        wheelFriction.stiffness = stiffness;
    }

    //Draws a Gizmo for the center of mass on the rigidbody
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere((transform.position + transform.rotation * centerOfMassPos), .05f);
    }

}
