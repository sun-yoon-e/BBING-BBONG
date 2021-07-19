using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gadd420;
public class Speedometer : MonoBehaviour
{
    ThirdPersonCamera cam;
    public RB_Controller controller;

    public Text mph;
    public Text gear;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<ThirdPersonCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //When the camera look at has been set get the RBController from the selected bike
        if (cam.lookAt && !controller)
        {
            controller = cam.lookAt.GetComponent<RB_Controller>();
        }

        //If the RB Controller is assigned
        if (controller)
        {
            //Gets and displays speed in Mph
            float speed = controller.currentSpeed * controller.msToMph;
            speed = Mathf.Round(speed);
            mph.text = (speed + " Mph");

            //Gets and displays the gears
            float currentGear = controller.currentGear + 1;
            gear.text = ("Gear " + currentGear);
        }
    }
}