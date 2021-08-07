using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelieHelp : MonoBehaviour
{
    public WheelCollider rearWheel;
    public WheelCollider frontWheel;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rearWheel.isGrounded && !frontWheel.isGrounded)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
        }
    }
    private void FixedUpdate()
    {
        
        if (rearWheel.isGrounded)
        {
            if (rb.transform.eulerAngles.x < 330 && rb.transform.eulerAngles.x > 250)
            {
                Debug.LogError(rb.transform.eulerAngles.x);
                rb.AddTorque(Vector3.right * 500);
            }
        }
    }
}
