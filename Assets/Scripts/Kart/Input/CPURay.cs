using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPURay
{
    public float raysHeightOffset;
    public float rayBackOffset;
    public CPURay(GameObject self, Quaternion direction, float maxDistance, float raysHeightOffset, float rayBackOffset)
    {
        this.self = self;
        this.direction = direction;
        maxSearchDistance = maxDistance;
        this.raysHeightOffset = raysHeightOffset;
        this.rayBackOffset = rayBackOffset;
    }

    public GameObject self;

    public Quaternion direction;

    public float maxSearchDistance;

    int advQuality = 5;
    public float AdvSearch(ref List<Vector3> gizmos)
    {

        float search = Search();

        float distanceOut = maxSearchDistance / 2f;
        float lowerOut = 0;
        for(int i = 0; i < advQuality; i++)
        {
            RaycastHit[] hitinfo = Physics.RaycastAll(self.transform.position + self.transform.up * raysHeightOffset - self.transform.forward * rayBackOffset + self.transform.rotation * (direction * Vector3.forward) * distanceOut, self.transform.rotation * (direction * Vector3.down), 1);
            foreach(RaycastHit hit in hitinfo)
            {
                if (hit.collider.gameObject.layer != LayerMask.GetMask("OutOfBounds") )
                {
                    lowerOut = distanceOut;
                    distanceOut += ((maxSearchDistance - distanceOut) / 2f);
                    
                    continue;
                }
            }

            distanceOut -= ((distanceOut - lowerOut) / 2f);
        }
        gizmos.Add(self.transform.position + self.transform.up * raysHeightOffset - self.transform.forward * rayBackOffset + self.transform.rotation * (direction * Vector3.forward) * distanceOut);

        if (search == -1 && distanceOut >= Mathf.Pow(.5f,advQuality - 1) * maxSearchDistance)
        {
            Debug.Log(-1);
            return -1;
        }
        else
        {
            if (search == -1)
            {
                Debug.Log(distanceOut);
                return distanceOut;
            }
        }
        Debug.Log(Mathf.Min(search, distanceOut));
        return Mathf.Min(search, distanceOut);
    }

    //returns -1 if nothing is found
    public float Search()
    {
        RaycastHit[] hitinfo = Physics.RaycastAll(self.transform.position + self.transform.up * raysHeightOffset - self.transform.forward * rayBackOffset, self.transform.rotation * (direction * Vector3.forward), maxSearchDistance);

        if(hitinfo.Length == 0)
        {
            return -1f;
        }
        else
        {
            for (int i = 0; i < hitinfo.Length; i++)
            {
                if (hitinfo[i].collider.isTrigger)
                {
                    if (hitinfo[i].collider.GetComponentInParent<Kart>() != null)
                    {
                        if (hitinfo[i].collider.GetComponentInParent<Kart>().gameObject != self)
                        {
                            return hitinfo[i].distance;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

                if(hitinfo[i].collider.GetComponentInParent<Kart>() != null)
                {
                    if (hitinfo[i].collider.GetComponentInParent<Kart>().gameObject != self)
                    {
                        return hitinfo[i].distance;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    return hitinfo[i].distance;
                }
                
            }
            return -1f;
        }
    }

    public Vector3 SearchNormal()
    {
        RaycastHit[] hitinfo = Physics.RaycastAll(self.transform.position + self.transform.up * raysHeightOffset - self.transform.forward * rayBackOffset, self.transform.rotation * (direction * Vector3.forward), maxSearchDistance);

        if (hitinfo.Length == 0)
        {
            return Vector3.up;
        }
        else
        {
            for (int i = 0; i < hitinfo.Length; i++)
            {
                if (hitinfo[i].collider.isTrigger)
                {
                    if (hitinfo[i].collider.GetComponentInParent<Kart>() != null)
                    {
                        if (hitinfo[i].collider.GetComponentInParent<Kart>().gameObject != self)
                        {
                            return hitinfo[i].normal;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

                if (hitinfo[i].collider.GetComponentInParent<Kart>() != null)
                {
                    if (hitinfo[i].collider.GetComponentInParent<Kart>().gameObject != self)
                    {
                        return hitinfo[i].normal;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    return hitinfo[i].normal;
                }

            }
            return Vector3.up;
        }
    }

}
