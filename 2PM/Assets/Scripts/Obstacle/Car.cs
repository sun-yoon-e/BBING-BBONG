using Gadd420;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    //ObstacleGenerator obstacle;
    NavMeshAgent agent;

    private void Start()
    {
        gameObject.layer = 20;

        //obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        if (gameClient.client_host)
            agent = transform.parent.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        if (gameClient.client_host)
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, 180.0f, transform.eulerAngles.z);
        else
            transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "AI")
        {
            if (gameClient.client_host)
                agent.isStopped = true;

            if (collision.gameObject.tag == "Player")
            {
                var item = collision.gameObject.GetComponent<Item>();
                var rb_controller = collision.gameObject.GetComponent<RB_Controller>();
                rb_controller.maxSpeed = item.orMaxSpeed / 2;
                item.isSlow = true;
            }
            if (collision.gameObject.tag == "AI")
            {
                //var item = collision.gameObject.GetComponent<AIItem>();
                //var rb_controller = collision.gameObject.GetComponent<AIRBController>();
                //rb_controller.maxSpeed = item.orMaxSpeed / 2;
                //item.isSlow = true;
            }

            Rigidbody targetRB = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 inNormal = Vector3.Normalize(transform.position - collision.gameObject.transform.position);
            Vector3 bounceVector = Vector3.Reflect(collision.relativeVelocity, inNormal);

            int bounceScale = 3;

            bounceVector.y = 0;
            
            if(bounceVector.x > 0 && bounceVector.x < bounceScale)
                bounceVector.x = bounceScale;
            else if(bounceVector.x > -bounceScale && bounceVector.x < 0)
                bounceVector.x = -bounceScale;

            if(bounceVector.x > bounceScale + 3)
                bounceVector.x = bounceScale + 3;
            else if (bounceVector.x < -bounceScale - 3)
                bounceVector.x = -bounceScale - 3;

            //if (bounceVector.y < 0)
            //    bounceVector.y = 0;
            //else if (bounceVector.y > 3)
            //    bounceVector.y = 3;

            if (bounceVector.z > 0 && bounceVector.z < bounceScale)
                bounceVector.z = bounceScale;
            else if (bounceVector.z > -bounceScale && bounceVector.z < 0)
                bounceVector.z = -bounceScale;

            if(bounceVector.z > bounceScale + 3)
                bounceVector.z = bounceScale + 3;
            else if (bounceVector.z < -bounceScale - 3)
                bounceVector.z = -bounceScale - 3;

            targetRB.AddForce(bounceVector, ForceMode.VelocityChange);
        }
    }
}