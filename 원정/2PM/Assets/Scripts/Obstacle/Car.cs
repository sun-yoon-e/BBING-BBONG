using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Car : MonoBehaviour
{
    ObstacleGenerator obstacle;
    Rigidbody rb;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, 180.0f, transform.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" /*|| collision.gameObject.tag == "mapBoxCollider"*/ )
        {
            //obstacle.GenerateCar(false);
            //Destroy(transform.parent.gameObject);

            Rigidbody targetRB = collision.gameObject.GetComponent<Rigidbody>();

            //targetRB.velocity = new Vector3(0, 0, 0);

            float cal = collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z;
            print(cal);

            Vector3 inNormal = Vector3.Normalize(transform.position - collision.gameObject.transform.position);
            Vector3 bounceVector = Vector3.Reflect(collision.relativeVelocity, inNormal);

            targetRB.AddForce(bounceVector, ForceMode.VelocityChange);

        }
    }
}