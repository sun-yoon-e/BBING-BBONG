using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gadd420
{

    public class ThirdPersonCamera : MonoBehaviour
    {
        public float minYAngle = -20.0f;
        public float MaxYAngle = 80.0f;

        public Transform lookAt;
        Transform camTransform;
        public float distance = 5.0f;

        private float currentX = 0.0f;
        private float currentY = 45.0f;
        public float mouseSens = 1f;

        private void Start()
        {
            camTransform = transform;
        }

        private void Update()
        {
            //Gets Mouse input
            currentX += Input.GetAxis("Mouse X");
            currentY -= Input.GetAxis("Mouse Y");

            //Clamp Rotation Values
            currentY = Mathf.Clamp(currentY, minYAngle, MaxYAngle);
        }

        private void LateUpdate()
        {
            if (lookAt)
            {
                //Set Rotation and Postion
                Vector3 dir = new Vector3(0, 0, -distance);
                Quaternion rotation = Quaternion.Euler(currentY * mouseSens, currentX * mouseSens, 0 * Time.deltaTime);

                camTransform.position = lookAt.position + rotation * dir;
                camTransform.LookAt(lookAt.position);
            }
        }
    }
}
