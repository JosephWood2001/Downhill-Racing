using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float explosionForce = 10f;
    public float explodeImpactTreashold = 2f;
    public Vector3 explosionPoint = new Vector3();

    [Header("Refrences")]
    public GameObject FullBarrel;
    public GameObject Explodedbarrel;
    public float radius;
    public ParticleSystem particle;


    private void OnCollisionEnter(Collision collision)
    {
        
        if (Explodedbarrel.activeSelf)
        {
            return;
        }

        if(collision.relativeVelocity.magnitude > explodeImpactTreashold)
        {
            Explode();
        }


    }

    private Collider[] hitColliders;
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private Rigidbody temp;
    void Explode()
    {
        ParticleSystem particles = Instantiate(particle, transform.position + transform.rotation * explosionPoint,Quaternion.Euler(0,0,0));
        Destroy(particles.gameObject,10f);
        
        hitColliders = Physics.OverlapSphere(transform.position + transform.rotation * explosionPoint, radius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponent<Rigidbody>() != null)
            {
                temp = hitColliders[i].GetComponent<Rigidbody>();
            }
            else if (hitColliders[i].GetComponentInParent<Rigidbody>() != null)
            {
                temp = hitColliders[i].GetComponentInParent<Rigidbody>();
            }
            else
            {
                continue;
            }


            if (!rigidbodies.Contains(temp))
            {
                rigidbodies.Add(temp);
                temp.AddExplosionForce(explosionForce, transform.position + explosionPoint, radius, .1f, ForceMode.Impulse);
            }
            
        }

        FullBarrel.SetActive(false);
        Explodedbarrel.SetActive(true);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * explosionPoint, .1f);
    }
}
