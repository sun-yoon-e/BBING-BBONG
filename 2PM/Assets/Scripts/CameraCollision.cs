using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public GameObject camera;
    public float maxDis;
    public LayerMask layers;


    // Update is called once per frame
    void Update()
    {
        Vector3 dir = camera.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, maxDis, layers))
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            camera.GetComponent<Gadd420.ThirdPersonCamera>().distance = dis;
        }
        else
        {
            camera.GetComponent<Gadd420.ThirdPersonCamera>().distance = maxDis;
        }
    }
}
