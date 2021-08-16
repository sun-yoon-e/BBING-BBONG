using UnityEngine;

public class Car: MonoBehaviour
{
    ObstacleGenerator obstacle;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
    }
    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        //transform.localRotation = Quaternion.Euler(0, 180.0f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ObstacleGenerator.CarObject car = obstacle.Cars.Find(p => p.Car == transform.parent.gameObject);
            if (car != null)
            {
                //Debug.Log("Coll Car");
                GameClient.Instance.DestroyCar(car.ID);
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        obstacle.GenerateCar(false);
    //        Destroy(transform.parent.gameObject);
    //    }
    //}
}
