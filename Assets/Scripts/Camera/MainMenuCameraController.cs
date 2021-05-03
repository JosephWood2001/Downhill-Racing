using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    Transform objectToFollow;
    public Transform ObjectToFollow
    {
        get
        {
            return objectToFollow;
        }
        set
        {
            if (objectToFollow == null)
            {
                transform.position = value.position;
                transform.rotation = value.rotation;
            }

            objectToFollow = value;
        }
    }
    public float followSpeed = 10f;
    public float lookSpeed = 10f;



    private void FollowTransform()
    {
        
        transform.position = Vector3.Lerp(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, objectToFollow.rotation, lookSpeed * Time.deltaTime);

    }


    private void Update()
    {
        FollowTransform();
        
    }
}
