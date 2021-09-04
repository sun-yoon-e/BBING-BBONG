using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gadd420
{

    public class ThirdPersonCamera : MonoBehaviour
    {
        public float minYAngle;
        public float MaxYAngle;

        public Transform lookAt;
        Transform camTransform;
        public float distance;

        private float currentX = 0.0f;
        private float currentY = 45.0f;
        public float mouseSens;

        Vector3 currentRotation;
        Vector3 rotationSmoothVelocity = Vector3.zero;

        private void Start()
        {
            camTransform = transform;
        }

        private void LateUpdate()
        {
            //Gets Mouse input
            currentX += Input.GetAxis("Mouse X") * mouseSens;
            currentY -= Input.GetAxis("Mouse Y") * mouseSens;

            //Clamp Rotation Values
            currentY = Mathf.Clamp(currentY, minYAngle, MaxYAngle);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(currentY, currentX), ref rotationSmoothVelocity, 0.05f);
            transform.eulerAngles = currentRotation;

            //Vector3 increaseYPosition = new Vector3(0, 2f, 0);
            transform.position = lookAt.position - transform.forward * distance + transform.right * 0f;

            //transform.position = Vector3.MoveTowards(transform.position, lookAt.position, 0.5f);

            //if (lookAt)
            //{
            //    //Set Rotation and Postion
            //    Vector3 dir = new Vector3(0, 0, -distance);
            //    Quaternion rotation = Quaternion.Euler(currentY * mouseSens, currentX * mouseSens, 0 * Time.deltaTime);

            //    camTransform.position = lookAt.position + rotation * dir;
            //    camTransform.LookAt(lookAt.position);
            //}
        }
    }
}