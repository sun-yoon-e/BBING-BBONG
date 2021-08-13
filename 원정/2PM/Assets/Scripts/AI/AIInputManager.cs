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
        HZInput();
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

    protected virtual void HZInput()
    {
        hzInputTime = Mathf.Clamp(hzInputTime, 0, inputSmoothingTime);

        if (isPressS || PressD() || PressA())
        {
            if (PressD() || isPressS)
            {
                if (hzInput < 0)
                {
                    hzInputTime -= 2 * Time.deltaTime;
                    hzInput = -Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
                else
                {
                    hzInputTime += 1 * Time.deltaTime;
                    hzInput = Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
            }
            if (PressA())
            {
                if (hzInput > 0.01f)
                {
                    hzInputTime -= 2 * Time.deltaTime;
                    hzInput = Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
                else
                {
                    hzInputTime += 1 * Time.deltaTime;
                    hzInput = -Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
            }
        }
        else
        {
            if (hzInputTime > 0.01f)
            {
                hzInputTime -= 1 * Time.deltaTime;
                if (hzInput < 0)
                {
                    hzInput = -Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
                if (hzInput > 0)
                {
                    hzInput = Mathf.InverseLerp(0, inputSmoothingTime, hzInputTime);
                }
            }
            else
            {
                hzInputTime = 0;
                hzInput = 0;
            }
        }
    }

    bool PressW()
    {
        if (movementScript.cal > 4)
            return true;

        return false;
    }

    bool PressS()
    {
        //if (movementScript.direction.z < 0)
        //    return true;

        return false;
    }

    bool PressD()   //오른쪽
    {
        //if (movementScript.direction.x > 3
        //    || movementScript.rot.x > 15f)
        //    return true;

        return false;
    }

    bool PressA()   //왼쪽
    {
        //if (movementScript.direction.x < -3
        //    || movementScript.rot.x < -15f)
        //    return true;

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "mapBoxCollider")
        {
            movementScript.isLookAgent = true;
            //StartCoroutine("goBack");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "mapBoxCollider")
        {
            movementScript.isLookAgent = false;
        }
    }

    IEnumerator goBack()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementScript.direction), 0.1f);

        yield return new WaitForSeconds(1f);
    }

    
}