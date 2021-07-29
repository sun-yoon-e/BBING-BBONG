using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gadd420
{
    
    public class Input_Manager : MonoBehaviour
    {

        public bool combineLeanAndSteering;

        public float inputSmoothingTime = 0.5f;

        float vInputTime;
        float hzInputTime;

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
            vInput = 0;
            hzInput = 0;
        }

        void Update()
        {
            VerticalInput();
            HZInput();
            GetLeanValue();
            GetLeanBackValue();
        }

        

        protected virtual void VerticalInput()
        {
            vInputTime = Mathf.Clamp(vInputTime, 0, inputSmoothingTime);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.W))
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
                if (Input.GetKey(KeyCode.S))
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

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.D))
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
                if (Input.GetKey(KeyCode.A))
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

        protected virtual void GetLeanValue()
        {
            if (!combineLeanAndSteering)
            {
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        leanInput = -1;
                    }
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        leanInput = 1;
                    }
                }
                else
                {
                    leanInput = 0;
                }
            }
            else
            {
                leanInput = hzInput;
            }
        }

        protected virtual void GetLeanBackValue()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    wheelieInput = -1;
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    wheelieInput = 1;
                }
            }
            else
            {
                wheelieInput = 0;
            }
        }
    }
}
