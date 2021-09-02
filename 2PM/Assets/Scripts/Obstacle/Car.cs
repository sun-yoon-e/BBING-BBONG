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
        //obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        agent = transform.parent.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles.x, 180.0f, transform.eulerAngles.z);

        //if (gameClient.isGameStarted && gameClient.client_host)
        //{
        //    if (gameObject != null)
        //        obstacle.MoveCar(gameObject.transform.parent.gameObject, transform.parent.position, transform.rotation.eulerAngles);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "AI")
        {
            agent.isStopped = true;

            if (collision.gameObject.tag == "Player")
            {
                var item = collision.gameObject.GetComponent<Item>();
                var rb_controller = collision.gameObject.GetComponent<RB_Controller>();
                rb_controller.maxSpeed = item.orMaxSpeed / 2;
                item.isSlow = true;

                //obstacle.DestroyCar(transform.parent.gameObject);
            }
            if (collision.gameObject.tag == "AI")
            {
                //var item = collision.gameObject.GetComponent<AIItem>();
                //var rb_controller = collision.gameObject.GetComponent<AIRBController>();
                //rb_controller.maxSpeed = item.orMaxSpeed / 2;
                //item.isSlow = true;

                //obstacle.DestroyCar(transform.parent.gameObject);
            }

            Rigidbody targetRB = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 inNormal = Vector3.Normalize(transform.position - collision.gameObject.transform.position);
            Vector3 bounceVector = Vector3.Reflect(collision.relativeVelocity, inNormal);
            bounceVector.y = 0;

            targetRB.AddForce(bounceVector, ForceMode.VelocityChange);
        }
    }
}