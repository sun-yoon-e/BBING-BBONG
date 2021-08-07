using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadd420;

public class PlayerLeaning : MonoBehaviour
{
    public float maxLeanForward = 0.5f;
    public float maxLeanBack = -0.3f;
    public float maxLeanSide = 0.45f;
    public float forwardLeanSmoothTime = 0.5f;
    public float rightLeanSmoothTime = 0.5f;
    Input_Manager inputs;



    float forwardPos;
    float rightPos;

    float forwardInput;
    float rightInput;

    float forwardT;
    float rightT;

    bool firstLeanFinished;
    bool firstLeanFinished2;

    float updateCount;
    
    // Start is called before the first frame update
    void Start()
    {
        inputs = GetComponentInParent<Input_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        //This is to trigger the leaning at the start otherwise when u first lean it snap left and this is the easy way to fix that
        if (!firstLeanFinished)
        {
            rightPos = -0.01f;
            forwardPos = -0.01f;
            
            firstLeanFinished = true;
        }

        //Stops the timer going above max time
        forwardT = Mathf.Clamp(forwardT,0, forwardLeanSmoothTime);
        rightT = Mathf.Clamp(rightT,0, rightLeanSmoothTime);

        //Get Inputs
        forwardInput = inputs.WheelieInput;
        rightInput = inputs.LeanInput;

        //If Leaning forward or back
        if (forwardInput !=0)
        {
            //Adds Timer direction depending on input
            forwardT += forwardInput * Time.deltaTime;

            //Get a value between 0-1 depending on timer completion
            float inverseT = Mathf.InverseLerp(0, forwardLeanSmoothTime, forwardT);

            //Lerps position depending on timer completion
            forwardPos = Mathf.Lerp(maxLeanBack, maxLeanForward, inverseT);

        }
        else
        {
            // If Follow Pos is currently forward
            if (forwardPos > 0)
            {
                //Count the timer down
                forwardT -= 1 * Time.deltaTime;

                float inverseT = Mathf.InverseLerp(0, forwardLeanSmoothTime, forwardT);
                forwardPos = Mathf.Lerp(maxLeanBack, maxLeanForward, inverseT);
            }
            if (forwardPos < 0)
            {
                //Adds to timer
                forwardT += 1 * Time.deltaTime;
                float inverseT = Mathf.InverseLerp(0, forwardLeanSmoothTime, forwardT);

                forwardPos = Mathf.Lerp(maxLeanBack,maxLeanForward, inverseT);
            }
            

        }




        if (rightInput != 0)
        {

            rightT += rightInput * Time.deltaTime;

            float inverseT = Mathf.InverseLerp(0, rightLeanSmoothTime, rightT);

            

            rightPos = Mathf.Lerp(-maxLeanSide, maxLeanSide, inverseT);
           

        }
        
        
        
        else
        {
           
            //If no inputs and leaning right
            if (rightPos > 0f)
            {
                //Count down the timer
                rightT -= 1 * Time.deltaTime;
                float inverseT = Mathf.InverseLerp(0, rightLeanSmoothTime, rightT);

                rightPos = Mathf.Lerp(-maxLeanSide, maxLeanSide, inverseT);
            }
            if (rightPos < 0f)
            {
                //Count up the timer
                rightT += 1 * Time.deltaTime;
                float inverseT = Mathf.InverseLerp(0, rightLeanSmoothTime, rightT);

                rightPos = Mathf.Lerp(-maxLeanSide, maxLeanSide, inverseT);
            }
            
            

        }

        //Waits 10 frames from start so you dont see the snapping at the start
        if (updateCount<10)
        {
            updateCount++;   
        }
        else
        {
            transform.localPosition = new Vector3(rightPos, transform.localPosition.y, forwardPos);
        }
    }

    
}

