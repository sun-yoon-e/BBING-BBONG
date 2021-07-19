using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadd420;

[RequireComponent(typeof(RB_Controller))]
public class PedalManager : MonoBehaviour
{

    public WheelRotationAxis pedalRotationAxis;
    RB_Controller rbScript;
    public GameObject pedalRotationOBJ;
    public float gearRatioInFirstGear = 2;
    public float gearRatioInLastGear = 10;
    public GameObject[] pedals;
    float ogRatioInFirst;
    Vector3 rotationSpeed;
    float gearRatioDifference;

    Input_Manager inputs;

    // Start is called before the first frame update
    void Start()
    {
        rbScript = GetComponent<RB_Controller>();
        inputs = GetComponent<Input_Manager>();
        gearRatioDifference = (gearRatioInFirstGear - gearRatioInLastGear) / (rbScript.numGears-1);
        ogRatioInFirst = gearRatioInFirstGear;

    }

    // Update is called once per frame
    void Update()
    {   //If first gear set torque
        if (rbScript.currentGear == 0)
        {
            gearRatioInFirstGear = ogRatioInFirst;
        }

        //If last gear set torque
        else if (rbScript.currentGear == rbScript.numGears - 1)
        {
            gearRatioInFirstGear = gearRatioInLastGear;
        }

        //Set the torque based on which gear you are in to simulate decrease in torque as you increase the gears
        else
        {
            gearRatioInFirstGear = ogRatioInFirst - gearRatioDifference * rbScript.currentGear;
        }
        float wheelRPM = rbScript.wheelColliders[0].rpm;

        if (inputs.VInput > 0 && rbScript.isGrounded)
        {
            if (pedalRotationAxis == WheelRotationAxis.X)
            {
                rotationSpeed = (Vector3.right * wheelRPM * 6 * Time.deltaTime) / gearRatioInFirstGear;
            }
            if (pedalRotationAxis == WheelRotationAxis.Y)
            {
                rotationSpeed = (Vector3.up * wheelRPM * 6 * Time.deltaTime) / gearRatioInFirstGear;
            }
            if (pedalRotationAxis == WheelRotationAxis.Z)
            {
                rotationSpeed = (Vector3.forward * wheelRPM * 6 * Time.deltaTime) / gearRatioInFirstGear;
            }


            pedalRotationOBJ.transform.Rotate((Vector3.right * wheelRPM * 6 * Time.deltaTime) / gearRatioInFirstGear);
            for (int i = 0; i < pedals.Length; i++)
            {
                pedals[i].transform.Rotate(-rotationSpeed);
            }
        }

        

    }
}
