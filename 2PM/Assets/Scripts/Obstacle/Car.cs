using Gadd420;
using UnityEngine;

public class Car : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

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

        if (gameClient.isGameStarted && gameClient.client_host)
        {
            if (gameObject != null)
                obstacle.MoveCar(gameObject.transform.parent.gameObject, transform.parent.position, transform.rotation.eulerAngles);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var item = collision.gameObject.GetComponent<Item>();
            var rb_controller = collision.gameObject.GetComponent<RB_Controller>();
            rb_controller.maxSpeed = item.orMaxSpeed / 2;
            item.isSlow = true;

            obstacle.DestroyCar(transform.parent.gameObject);
        }
        if (collision.gameObject.tag == "AI")
        {
            //var item = collision.gameObject.GetComponent<AIItem>();
            //var rb_controller = collision.gameObject.GetComponent<AIRBController>();
            //rb_controller.maxSpeed = item.orMaxSpeed / 2;
            //item.isSlow = true;

            obstacle.DestroyCar(transform.parent.gameObject);
        }
    }
}