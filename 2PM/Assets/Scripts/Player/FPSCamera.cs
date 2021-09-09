using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public Camera cam;
    public float mouseSensitivity = 10.0f;
    public float cameraRotationLimitX = 30.0f;
    public float cameraRotationLimitY = 70.0f;
    private float cameraRotationX;
    private float cameraRotationY;

    // Update is called once per frame
    void Update()
    {
        if (gameTimer.instance.isStart)
        {
            CameraRotate();
        }
    }
    
    void CameraRotate()
    {
        //카메라 회전 
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -cameraRotationLimitX, cameraRotationLimitX);
        cameraRotationY += mouseX;
        cameraRotationY = Mathf.Clamp(cameraRotationY, -cameraRotationLimitY, cameraRotationLimitY);

        cam.transform.localEulerAngles = new Vector3(cameraRotationX, cameraRotationY, 0f);
    }
}
