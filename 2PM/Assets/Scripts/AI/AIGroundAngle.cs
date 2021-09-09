using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadd420;
public class AIGroundAngle : MonoBehaviour
{
    [Header("Script Not Used If Safe Wheelies Is Turned Off")]

    public LayerMask layerMask;

    AIRBController rbScript;
    AIAutoLeveling autoLeveler;

    public GameObject frontRayPos;
    public GameObject rearRayPos;

    bool upHill;

    [HideInInspector] public float forwardY;

    [HideInInspector] public float surfaceAngle;

    [HideInInspector] public float actualAngle;

    [HideInInspector] public float wheelieAngleMinusSurfaceAngle;

    // Start is called before the first frame update
    void Start()
    {
        rbScript = GetComponent<AIRBController>();
        autoLeveler = GetComponent<AIAutoLeveling>();

        if (autoLeveler.safeWheelies)
        {
            rearRayPos.transform.position = rbScript.wheels[0].transform.position;
            frontRayPos.transform.position = rbScript.wheels[1].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autoLeveler.safeWheelies)
        {
            SetRayPositionsAndRotations();
            RayCastsAndUphillCheck();
            CalculateForwardY();
        }
    }

    void SetRayPositionsAndRotations()
    {
        float heightDifference = frontRayPos.transform.position.y - rearRayPos.transform.position.y;

        frontRayPos.transform.position = new Vector3(rbScript.wheelColliders[1].transform.position.x, frontRayPos.transform.position.y - heightDifference, rbScript.wheelColliders[1].transform.position.z);

        rearRayPos.transform.rotation = Quaternion.Euler(-rbScript.transform.rotation.x, 0, 0);
        frontRayPos.transform.rotation = Quaternion.Euler(-rbScript.transform.rotation.x, 0, 0);
    }

    void RayCastsAndUphillCheck()
    {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rearRayPos.transform.position, rearRayPos.transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(rearRayPos.transform.position, rearRayPos.transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
            surfaceAngle = Vector3.Angle(hit.normal, -Vector3.up);
        }
        else
        {
            Debug.DrawRay(rearRayPos.transform.position, rearRayPos.transform.TransformDirection(-Vector3.up) * 1000, Color.white);
        }

        RaycastHit forwardHit;

        if (Physics.Raycast(frontRayPos.transform.position, frontRayPos.transform.TransformDirection(-Vector3.up), out forwardHit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(frontRayPos.transform.position, frontRayPos.transform.TransformDirection(-Vector3.up) * forwardHit.distance, Color.yellow);
        }
        else
        {
            upHill = true;
        }

        if (forwardHit.distance < hit.distance)
        {
            upHill = true;
        }
        else
        {
            upHill = false;
        }
    }

    void CalculateForwardY()
    {
        if (upHill)
        {
            actualAngle = 180 - surfaceAngle;
            forwardY = Mathf.InverseLerp(0, 45, actualAngle);
            wheelieAngleMinusSurfaceAngle = 360 - rbScript.transform.localEulerAngles.x - actualAngle;
        }
        else
        {
            actualAngle = 180 - surfaceAngle;
            forwardY = -Mathf.InverseLerp(0, 45, actualAngle);
            wheelieAngleMinusSurfaceAngle = 360 - rbScript.transform.localEulerAngles.x + actualAngle;
        }

        forwardY = Mathf.Clamp(forwardY, -1, 0.5f);
    }

}



