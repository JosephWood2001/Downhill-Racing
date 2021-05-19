
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInputSender : InputSender
{
    private CPURay[] rays;
    private CPURay searchForSurface;
    private Path path;

    public float timeToReset = 1.5f;
    private float lastTimeAboveThreshold = 0f;
    public float speedThreshold = .5f;

    public float curveIntensity = .25f;
    public float speedLimitIntensity = 6;
    public float handelingSpeedLimitIntensity = .5f;

    public float targetDistantToRacingLine = 1f;

    public int raysCount = 10;
    public float raysHeightOffset = .1f;
    public float viewingAngle = 270f;
    public float rayLengthForward = 2f;
    public float rayLengthSideways = 1f;
    public float rayBackOffsetForward = 0;
    public float rayBackOffsetSideways = .75f;

    int seed;
    private void Start()
    {
        seed = Random.Range(0, 10000000);

        rays = new CPURay[raysCount];
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = new CPURay(base.kart.gameObject, Quaternion.Euler(0, viewingAngle / (raysCount - 1) * i - (viewingAngle/2f), 0), Mathf.Lerp(rayLengthSideways, rayLengthForward,Mathf.Pow( Mathf.Cos( Mathf.Deg2Rad * (viewingAngle / (raysCount - 1) * i - (viewingAngle / 2f))),3f) ) , raysHeightOffset, Mathf.Lerp(rayBackOffsetSideways, rayBackOffsetForward, Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * (viewingAngle / (raysCount - 1) * i - (viewingAngle / 2f))), 3f)));
        }

        searchForSurface = new CPURay(base.kart.gameObject, Quaternion.Euler(90, 0, 0), 10f, 0, 0);

        path = GameObject.FindGameObjectWithTag("Path").GetComponent<Path>();

        lastTimeAboveThreshold = Time.time;
    }

    private Vector3 vectorToRaceLine = Vector3.zero;// for gizmo debug
    protected override void Update() //NOTE: may work better if in a fixed update
    {
        base.Update();

        if (kart.ParkingBreak)
        {
            lastTimeAboveThreshold = Time.time;
        }
        else
        {
            if(kart.kartRB.velocity.magnitude < speedThreshold && lastTimeAboveThreshold + timeToReset < Time.time)
            {
                lastTimeAboveThreshold = Time.time;
                reset = true;
            }
            else if(kart.kartRB.velocity.magnitude > speedThreshold)
            {
                lastTimeAboveThreshold = Time.time;
            }
        }

        float[] shortCheck = new float[raysCount];
        gizmos = new List<Vector3>();
        for (int i = 0; i < shortCheck.Length; i++)
        {
            shortCheck[i] = rays[i].AdvSearch(ref gizmos);
        }

        path.GetAIData(kart.transform.position, kart.transform.right, 15f, 30f, seed, out bool right, out float distanceToPoint, out Vector3 vectorToLine, out float offAngleRightValue, out float offAngleRightFarValue, out float offAngleRightLongValue);
        vectorToRaceLine = vectorToLine; // for gizmo debug

        vertical = 1f;
        horizontal = 0f;

        for (int i = 0; i < shortCheck.Length / 2; i++)
        {
            if (shortCheck[i] > 0)
            {
                horizontal += (1f - shortCheck[i] / 2);
            }
        }

        for (int i = shortCheck.Length / 2; i < shortCheck.Length; i++)
        {
            if (shortCheck[i] > 0)
            {
                horizontal -= (1f - shortCheck[i] / 2);
            }
        }

        if (right)
        {
            horizontal += Mathf.Clamp(Mathf.Clamp(distanceToPoint - targetDistantToRacingLine,0,1000) / 3, 0f, 1f);
        }
        else
        {
            horizontal -= Mathf.Clamp(Mathf.Clamp(distanceToPoint - targetDistantToRacingLine, 0, 1000) / 3, 0f, 1f);

        }

        horizontal -= offAngleRightValue;

        
        if(kart.kartRB.velocity.magnitude > Mathf.Pow(Mathf.Pow(kart.Handeling, handelingSpeedLimitIntensity) / (offAngleRightFarValue + offAngleRightLongValue),curveIntensity) * speedLimitIntensity)
        {
            breaking = 1f;
        }
        else
        {
            breaking = 0f;
        }

        if (!kart.isGrounded)
        {
            Vector3 normalRelitive = Quaternion.Inverse(kart.transform.rotation) * searchForSurface.SearchNormal();
            
            if (normalRelitive.z > 0)
            {
                airVertical = 1;
            }
            else
            {
                airVertical = -1;
            }

            if (normalRelitive.x > 0)
            {
                airRoll = 1;
            }
            else
            {
                airRoll = -1;
            }

            airHorizontal = horizontal;

        }

    }

    protected override void CreateInput()
    {
        base.CreateInput();
    }

    List<Vector3> gizmos = new List<Vector3>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < rays.Length; i++)
        {
            Gizmos.DrawLine(kart.gameObject.transform.position + kart.transform.up * raysHeightOffset - kart.transform.forward * rays[i].rayBackOffset, kart.gameObject.transform.position + kart.transform.up * raysHeightOffset - kart.transform.forward * rays[i].rayBackOffset + kart.transform.rotation * (rays[i].direction * Vector3.forward) * rays[i].maxSearchDistance);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(kart.gameObject.transform.position, kart.gameObject.transform.position + vectorToRaceLine);

        foreach(Vector3 gizmo in gizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(gizmo, .1f);
        }

    }
}
