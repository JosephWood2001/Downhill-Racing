using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public bool isSteer;
    public bool isSteerMesh; //if the mesh should rotate on steer or not
    public bool inverseSteer;
    public bool isMotor;
    public bool isBreak;
    [Header("Wheel Stats")]
    public float motorTorque;
    public float steeringAngle;
    public float breakingTorque;

    [HideInInspector]
    public WheelCollider wheelCollider;
    private Transform wheelTransform; //transform that contains the mesh of the wheel
    private void OnEnable()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>();
        //custom settings for the wheelColliders
        wheelCollider.ConfigureVehicleSubsteps(1000, 20, 20);
    }

    private void Start()
    {
        wheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (!wheelCollider.isGrounded)
        {
            wheelCollider.wheelDampingRate = 10000000f;
        }
        else
        {
            wheelCollider.wheelDampingRate = 0.00001f;
        }
    }

    /**Applies steering if isSteer
     * 
     */
    public void Steer(float steerInput)
    {
        if (isSteer)
        {
            wheelCollider.steerAngle = steerInput * steeringAngle;
        }

    }

    /**Applies motor torque to wheelCollider
     * 
     */
    public void Accelerate(bool isRunning, float verticalInput)
    {
        if (isRunning)
        {
            wheelCollider.motorTorque = motorTorque * verticalInput;
        }
        else
        {
            wheelCollider.motorTorque = 0;
        }
    }

    /**Applies break torque to wheelCollider
     * 
     */
    public void ApplyBreaks(float breaking)
    {
        wheelCollider.brakeTorque = breaking * breakingTorque;
    }

    /**updates wheel's transform to their respective poses from their wheel colliders
     */
    public void UpdateWheelPose()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
        //wheelTransform.position = pos;
        if (isSteerMesh)
        {
            wheelTransform.rotation = quat;
        }
        else
        {
            quat = Quaternion.Inverse(transform.rotation) * quat;
            wheelTransform.localRotation = Quaternion.Euler(quat.eulerAngles.x, 0, 0);
        }


    }

    public WheelCollider GetWheelCollider()
    {
        return wheelCollider;
    }

    public void SetWheelFrictionCurve(WheelFrictionCurve wheelFriction)
    {
        this.wheelCollider.sidewaysFriction = wheelFriction;
    }


}
