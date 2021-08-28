using Gadd420;
using UnityEngine;

public class Car : MonoBehaviour
{
    ObstacleGenerator obstacle;
    Rigidbody rb;

    Vector3 myPrevPosition;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        //transform.localRotation = Quaternion.Euler(0, 180.0f, 0);

        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;

        if (GameClient.Instance.isGameStarted && GameClient.Instance.client_host && myPrevPosition != rb.position)// || myPrevEulerAngles != rb.rotation.eulerAngles)
        {
            if (gameObject != null)
                obstacle.MoveCar(transform.parent.gameObject, transform.parent.gameObject.transform.position);//);
            myPrevPosition = transform.parent.gameObject.transform.position;
            //myPrevEulerAngles = rb.rotation.eulerAngles;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "AI")
        {
            var item = collision.gameObject.GetComponent<Item>();
            var rb_controller = collision.gameObject.GetComponent<RB_Controller>();
            rb_controller.maxSpeed = item.orMaxSpeed / 2;
            item.isSlow = true;
            //obstacle.GenerateCar();
            //Destroy(transform.parent.gameObject);

            obstacle.DestroyCar(transform.parent.gameObject);
        }
    }
}