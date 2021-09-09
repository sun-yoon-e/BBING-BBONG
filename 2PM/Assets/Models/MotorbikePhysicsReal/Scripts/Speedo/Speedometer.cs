using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gadd420;
public class Speedometer : MonoBehaviour
{
    ThirdPersonCamera cam;
    public RB_Controller controller;

    public Text kmh;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //When the camera look at has been set get the RBController from the selected bike
        if (!controller)
        {
            controller = cam.lookAt.GetComponent<RB_Controller>();
        }

        //If the RB Controller is assigned
        if (controller)
        {
            //Gets and displays speed in Mph
            float speed = controller.currentSpeed;
            kmh.color = new Color(1f, 0.92f - (speed * 0.015f), 0f);
            speed = Mathf.Round(speed);
            kmh.text = (speed + "");
        }
    }
}