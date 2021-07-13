using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorControls : MonoBehaviour
{

    public Camera cam;
    public float speed = 1f;
    public float dampen = 5f;
    public float sensitivity = 1f;

    Vector3 velocity = Vector3.zero;

    void Update()
    {
        //mouse movements
        if (Input.GetKey(KeyCode.Mouse2))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
            
            transform.Rotate(new Vector3(0, mouseX, 0), Space.World);
            
            transform.Rotate(new Vector3(-mouseY, 0, 0));

            float yAngle = transform.localRotation.eulerAngles.x;
            if (yAngle > 180f)
            {
                yAngle = yAngle - 360f;
            }
            transform.localRotation = Quaternion.Euler(Mathf.Clamp(yAngle, -80f, 80f), transform.rotation.eulerAngles.y, 0);
        }
        
        //end

        //key movements
        velocity.x += Input.GetAxis("Horizontal") * speed * Mathf.Clamp(transform.position.y,1f, 10000f);
        velocity.z += Input.GetAxis("Vertical") * speed * Mathf.Clamp(transform.position.y, 1f, 10000f);
        velocity.y += Input.GetAxis("AirHorizontal") * speed * Mathf.Clamp(transform.position.y, 1f, 10000f);
        transform.Translate(Quaternion.Euler(0,transform.rotation.eulerAngles.y,0) * velocity * Time.deltaTime,Space.World);
        //end

    }

    private void FixedUpdate()
    {
        velocity.x /= dampen;
        velocity.y /= 2 * dampen;
        velocity.z /= dampen;
    }

    public Vector3 GetLookingAtPoint()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, 1000f);
        return hit.point;
    }

}
