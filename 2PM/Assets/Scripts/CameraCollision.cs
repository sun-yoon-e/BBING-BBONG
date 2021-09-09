using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public GameObject Camera;
    public float maxDis;
    public LayerMask layers;

    void Update()
    {
        Vector3 dir = Camera.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, maxDis, layers))
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            Camera.GetComponent<Gadd420.ThirdPersonCamera>().distance = dis;
        }
        else
            Camera.GetComponent<Gadd420.ThirdPersonCamera>().distance = maxDis;
    }
}