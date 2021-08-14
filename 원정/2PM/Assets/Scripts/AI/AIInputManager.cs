using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Gadd420;

public class AIInputManager : MonoBehaviour
{
    AIMovement movementScript;

    public bool combineLeanAndSteering;

    public float inputSmoothingTime = 0.5f;

    float vInputTime;
    float hzInputTime;

    bool isPressS = false;

    //A&D
    protected float hzInput;
    public float HzInput
    {
        get { return hzInput; }
    }

    //W&S
    protected float vInput;
    public float VInput
    {
        get { return vInput; }
    }

    //LMouse & RMouse
    protected float leanInput;
    public float LeanInput
    {
        get { return leanInput; }
    }

    //LShift && LCtrl
    protected float wheelieInput;
    public float WheelieInput
    {
        get { return wheelieInput; }
    }

    private void Start()
    {
        movementScript = GetComponent<AIMovement>();

        vInput = 0;
        hzInput = 0;
    }

    void Update()
    {
        VerticalInput();
    }

    protected virtual void VerticalInput()
    {
        vInputTime = Mathf.Clamp(vInputTime, 0, inputSmoothingTime);

        if (PressW() || PressS() || isPressS)
        {
            if (PressW())
            {
                if (vInput < 0)
                {
                    vInputTime -= 2 * Time.deltaTime;
                    vInput = -Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
                else
                {
                    vInputTime += 1 * Time.deltaTime;
                    vInput = Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
            }
            if (PressS() || isPressS)
            {
                if (vInput > 0.01f)
                {
                    vInputTime -= 2 * Time.deltaTime;
                    vInput = Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
                else
                {
                    vInputTime += 1 * Time.deltaTime;
                    vInput = -Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
            }
        }
        else
        {
            if (vInputTime > 0.01f)
            {
                vInputTime -= 1 * Time.deltaTime;
                if (vInput < 0)
                {
                    vInput = -Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
                if (vInput > 0)
                {
                    vInput = Mathf.InverseLerp(0, inputSmoothingTime, vInputTime);
                }
            }
            else
            {
                vInputTime = 0;
                vInput = 0;
            }
        }
    }

    bool PressW()
    {
        if (movementScript.cal > 5 && movementScript.isArriveDestination == false)
            return true;

        return false;
    }

    bool PressS()
    {
        //if (movementScript.agent.isStopped)
        //    return true;

        return false;
    }
}