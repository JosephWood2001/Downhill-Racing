using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingLine : MonoBehaviour
{
    public RacingLineSegment[] lineSegments = new RacingLineSegment[1];
    public float[] bakedDistance;

    private void Awake()
    {
        BakeDistance();
    }

    // Distance is one unit per line segment
    public float FindDistanceAlongRaceLine(Vector3 point)
    {
        float closestDistance = Vector3.Distance(point, lineSegments[0].point1);
        float tempClosestDistance;
        float distanceAtClosestDistance = 0f;
        for (int i = 0; i < lineSegments.Length; i++)
        {
            Vector3 lineVector = lineSegments[i].point2 - lineSegments[i].point1;
            Vector3 pointVector = point - lineSegments[i].point1;
            float percentAlongLine = Vector3.Dot(lineVector, pointVector) / Vector3.Dot(lineVector, lineVector);
            if(percentAlongLine >= 1)
            {
                tempClosestDistance = Vector3.Distance(point, lineSegments[i].point2);
                
            }else if(percentAlongLine <= 0)
            {
                tempClosestDistance = Vector3.Distance(point, lineSegments[i].point1);
            }
            else
            {
                tempClosestDistance = Vector3.Distance( Vector3.Lerp(lineSegments[i].point1, lineSegments[i].point2, percentAlongLine), point);
            }
            if (tempClosestDistance < closestDistance)
            {
                closestDistance = tempClosestDistance;
                distanceAtClosestDistance = (float)i + Mathf.Clamp(percentAlongLine, 0f, 1f);
            }
        }
        return distanceAtClosestDistance;
    }

    public void AddPoint(Vector3 newPoint)
    {
        RacingLineSegment[] newLineSegments = new RacingLineSegment[lineSegments.Length + 1];
        for (int i = 0; i < lineSegments.Length; i++)
        {
            newLineSegments[i] = lineSegments[i];
        }
        newLineSegments[newLineSegments.Length - 1] = new RacingLineSegment(lineSegments[lineSegments.Length - 1].point2, newPoint);
        lineSegments = newLineSegments;
    }

    public void MovePoint(int index, Vector3 newPosition)
    {
        if (index < lineSegments.Length)
        {
            lineSegments[index].point1 = newPosition;
        }
        if(index > 0)
        {
            lineSegments[index - 1].point2 = newPosition;
        }
        
    }

    public void BakeDistance()
    {
        bakedDistance = new float[lineSegments.Length + 1];
        bakedDistance[0] = 0;
        for (int i = 0; i < lineSegments.Length; i++)
        {
            bakedDistance[i + 1] = Vector3.Distance(lineSegments[i].point1, lineSegments[i].point2) + bakedDistance[i];
        }
    }

    int tempInt;
    float tempFloat;
    public float GetUnitDistanceAlongCurve(Vector3 point)
    {
        float distance = FindDistanceAlongRaceLine(point);

        return GetUnitDistanceFromDistance(distance);
    }

    private float GetUnitDistanceFromDistance(float distance)
    {
        tempInt = Mathf.FloorToInt(distance);
        tempFloat = distance - tempInt;
        if (tempFloat > 0)
        {
            return Mathf.Lerp(bakedDistance[tempInt], bakedDistance[tempInt + 1], tempFloat);
        }
        else
        {
            return bakedDistance[tempInt];
        }
    }

    public void GetAIData(Vector3 point, Vector3 pointRight, float farDist, float longDist ,out bool right, out float distanceToPoint, out Vector3 vectorToLine, out float offAngleRightValue, out float offAngleRightFarValue, out float offAngleRightLongValue)
    {
        float distanceAlongLine = FindDistanceAlongRaceLine(point);
        float unitDist = GetUnitDistanceFromDistance(distanceAlongLine);
        Vector3 targetPoint;
        if(Mathf.FloorToInt(distanceAlongLine) != lineSegments.Length)
        {
            targetPoint = Vector3.Lerp(lineSegments[Mathf.FloorToInt(distanceAlongLine)].point1, lineSegments[Mathf.FloorToInt(distanceAlongLine)].point2, distanceAlongLine - Mathf.FloorToInt(distanceAlongLine));
        }
        else
        {
            targetPoint = lineSegments[lineSegments.Length-1].point2;
        }
        distanceToPoint = Vector3.Distance(targetPoint,point);
        vectorToLine = targetPoint - point;
        right = (Vector3.Dot(pointRight, vectorToLine) > 0);

        Vector3 tangent = (lineSegments[Mathf.Clamp(Mathf.FloorToInt(distanceAlongLine), 0, lineSegments.Length - 1)].point2 - lineSegments[Mathf.Clamp(Mathf.FloorToInt(distanceAlongLine), 0, lineSegments.Length - 1)].point1).normalized;

        offAngleRightValue = Vector3.Dot(tangent,-pointRight);

        float distanceAtFar = 0;
        for (int i = 0; i < bakedDistance.Length - 1; i++)
        {
            if (bakedDistance[i] < unitDist + farDist && bakedDistance[i + 1] > unitDist + farDist)
            {
                distanceAtFar = (i) + ((unitDist + farDist - bakedDistance[i]) / (bakedDistance[i + 1] - bakedDistance[i]));
                break;
            }
        }

        Vector3 tangentFar = GetPointAtDistance(distanceAtFar) - targetPoint;

        offAngleRightFarValue = 1f - Mathf.Clamp(Vector3.Dot(tangent.normalized, tangentFar.normalized),-1f,1f);


        float distanceAtLong = 0;
        for (int i = 0; i < bakedDistance.Length - 1; i++)
        {
            if (bakedDistance[i] < unitDist + farDist + longDist && bakedDistance[i + 1] > unitDist + farDist + longDist)
            {
                distanceAtLong = (i) + ((unitDist + farDist + longDist - bakedDistance[i]) / (bakedDistance[i + 1] - bakedDistance[i]));
                break;
            }
        }


        Vector3 tangentLong = GetPointAtDistance(distanceAtLong) - targetPoint;

        offAngleRightLongValue = 1f - Mathf.Clamp(Vector3.Dot(tangent.normalized, tangentLong.normalized),-1f, 1f);

    }

    public Vector3 GetPointAtDistance(float distance)
    {
        if(distance >= lineSegments.Length)
        {
            return lineSegments[lineSegments.Length - 1].point2;
        }
        else
        {
            return Vector3.Lerp(lineSegments[Mathf.FloorToInt(distance)].point1, lineSegments[Mathf.FloorToInt(distance)].point2, distance - (float)Mathf.FloorToInt(distance));
        }
    }

    public Vector3 GetClosestPointOnCurve(Vector3 point)
    {
        float distanceAlongLine = FindDistanceAlongRaceLine(point);
        Vector3 targetPoint = lineSegments[Mathf.FloorToInt(distanceAlongLine)].point1 + Vector3.Lerp(lineSegments[Mathf.FloorToInt(distanceAlongLine)].point1, lineSegments[Mathf.FloorToInt(distanceAlongLine)].point2, distanceAlongLine - Mathf.FloorToInt(distanceAlongLine));
        return targetPoint;
    }

    public Vector3 GetClosestPointTanget(Vector3 point)
    {
        float distanceAlongLine = FindDistanceAlongRaceLine(point);
        return (lineSegments[Mathf.FloorToInt(distanceAlongLine)].point2 - lineSegments[Mathf.FloorToInt(distanceAlongLine)].point1).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < lineSegments.Length; i++)
        {
            Gizmos.DrawSphere(lineSegments[i].point1, .1f);
            Gizmos.DrawLine(lineSegments[i].point1, lineSegments[i].point2);
        }
        Gizmos.DrawSphere(lineSegments[lineSegments.Length-1].point2, .1f);
       
    }
}
