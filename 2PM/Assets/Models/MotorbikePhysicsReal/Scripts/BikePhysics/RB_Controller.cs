using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gadd420
{
    [System.Serializable]
    public enum WheelRotationAxis { X = 0, Y = 1, Z =2};

    [RequireComponent(typeof(Rigidbody),typeof(AutoLeveling),typeof(CrashController))]
    [RequireComponent(typeof(Input_Manager),typeof(AudioSource),typeof(NitrousManager))]
    [RequireComponent(typeof(GroundAngle))]
    public class RB_Controller : MonoBehaviour
    {
        public WheelRotationAxis wheelRotationAxis;

        #region Variables

        public Vector3 myPrevPosition;
        public Vector3 myPrevEulerAngles;

        Rigidbody rb;
        CrashController crashScript;
        Input_Manager inputs;
        NitrousManager nitrousScript;
        PlayerCamera cameraScript;
        
        float maxLeanRight;
        float maxLeanLeft;

        [HideInInspector] public bool isGrounded;
        bool reversing;
        
        #region Engine Properties
        [Header("All angles are in Degrees and Speed in MPH")]
        [Space]
        [Header("Engine Properties")]
        public float firstGearTorque = 500;
        public float topGearTorque = 350;
        public bool isAutomatic;

        [Header("MPH")]
        public float maxSpeed = 70;
        public float maxReverseSpeed = 3f;
        
        //Metres a second
        //[HideInInspector] public float msToMph = 2.237f;
        [HideInInspector] public float currentSpeed;
        
        float rpm;

        [Space]
        [Header("Gears")]
        public int numGears = 6;
        public int currentGear = 1;

        //Read Only
        float ogTorque;
        float wheelRPM;
        float currentGearPercent;
        float torqueDifference;
        #endregion

        #region Brakes
        [Space]
        [Header("Brakes")]
        public float frontBrakeTorque = 350;
        public float backBrakeTorque = 350;
        #endregion

        #region Steering Properties

        [Space]
        [Header("Lean Values")]
        public float LeanSteerAmount = 2.5f;
        public float maxLeanAngle = 45;
        public float minLeanTorque = 0.1f;
        public float maxLeanTorque = 2;
        public float speedForMaxLean = 50;
        float currentLeanTorque;

        [Space]
        [Header("Steer Values")]
        public float minSteerAngle = 5;
        public float maxSteerAngle = 25;
        public float speedForMinSteer = 50;
        public float steerLeanTorque = 0.5f;

        [Space]
        [Header("Misc Properties")]
        public float wheelieTorque = 750f;
        public float backFlipTorque = 500f;
        public float inAirSpinTorque = 500f;

        [Space]
        #endregion

        #region Wheels and Steering Meshs
        [Header("Wheel Colliders")]
        public WheelCollider[] wheelColliders;

        [Space]

        [Header("Wheel Positions")]
        public GameObject[] wheels;

        [Space]

        [Header("Fork Pivot")]
        //Everything on the Front that Should Turn with the Wheel
        public GameObject forkPivot;
        #endregion

        [Space]

        #region Centre of Gravity
        [Header("Centre Of Gravity")]
        public Transform cog;
        #endregion

        [Space]

        #region Audio
        [Header("Audio")]
        public float lowPitch = 1f;
        public float highPitch = 3f;
        AudioSource sound;
        #endregion

        #endregion

        #region BuiltIn Methods
        void Start()
        {
            
            //Get Components
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
            sound = GetComponent<AudioSource>();
            crashScript = GetComponent<CrashController>();
            inputs = GetComponent<Input_Manager>();
            nitrousScript = GetComponent<NitrousManager>();
            cameraScript = GetComponent<PlayerCamera>();

            //Sets ogTorque which never changes
            ogTorque = firstGearTorque;

            //Sets the Centre of Gravity if one is assigned
            if (cog)
            {
                rb.centerOfMass = cog.localPosition;
            }
            //Sets Max lean angles in euler angles
            maxLeanLeft = 0 + maxLeanAngle;
            maxLeanRight = 360 - maxLeanAngle;

            if (!isAutomatic)
            {
                //Calculates torque to add or take away when shifting gears
                torqueDifference = (firstGearTorque - topGearTorque) / (numGears - 1);
            }
            else
            {
                //If its automatic the bike has 1 gear to save me writing extra code for the automatic rev sound
                numGears = 1;
            }

            //Converts speed in mph to ms (meters per second)
            //maxSpeed = maxSpeed / msToMph;
            //Converts reverse speed to ms
            //maxReverseSpeed = maxReverseSpeed / msToMph;
            //Converts speed for max lean to ms
            //speedForMaxLean = speedForMaxLean / msToMph;
            //Converts speed for min steer to ms
            //speedForMinSteer = speedForMinSteer / msToMph;
        }

        // Update is called once per frame
        void Update()
        {
            if (cameraScript.nowCam == 1)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                if (crashScript &&crashScript.isCrashed)
                {
                    //When Crashed the Z rotation is turned off so the bike can crash more realisticly
                    rb.constraints = RigidbodyConstraints.None;
                    transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                    crashScript.isCrashed = false;
                }
                else
                {
                    //If not Crashed Z rotation is Locked to stop gravity pulling the bike down
                    rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                }
            }

            //Checks when no wheels are touching the ground isGrounded is set to true
            if (!wheelColliders[0].isGrounded && !wheelColliders[1].isGrounded)
            {
                isGrounded = false;
            }
            else
            {
                isGrounded = true;
            }

            //Run Custom Methods
            HandleSteering();
            HandleLeanSpeed();
            HandleEngine();
            HandleBrakes();
            SetWheelMeshPos();

            if (myPrevPosition != rb.position || myPrevEulerAngles != rb.rotation.eulerAngles)
            {
                GameClient.Instance.UpdatePosition(rb.position, rb.rotation.eulerAngles);
                myPrevPosition = rb.position;
                myPrevEulerAngles = rb.rotation.eulerAngles;
            }
        }

        private void FixedUpdate()
        {
            //Calculate and Add Torque if you aren't crashed
            if (!crashScript.isCrashed)
            {
                CalculateTorque();
                AddTorque();
            }
            //Wheel torque is set to 0 and torque method is still run to stop the torque being stuck on what it was when you crashed
            else
            {
                firstGearTorque = 0;
                AddTorque();
            }
        }
        #endregion

        #region Custom Methods
        void HandleSteering()
        {
            //Lerp between max steer angle and min steer angle depending on the speed
            float steerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, currentSpeed / speedForMinSteer) * inputs.HzInput;
          
            //No steering if crashed
            if (crashScript.isCrashed)
            {
                steerAngle = 0;
            }

            //if you are leaning and steering the same way the lean steer will add to the normal steer value
            if (inputs.LeanInput != -inputs.HzInput)
            {
                //This is to stop extra steering being added if you are not leaning
                if (inputs.LeanInput != 0)
                {
                    steerAngle = steerAngle + (LeanSteerAmount * inputs.LeanInput);
                }
                
                //This is to stop the lean steer amount making you more than you should
                steerAngle = Mathf.Clamp(steerAngle, -maxSteerAngle, maxSteerAngle);
            }
            
            //Set Steer angle for collider
            wheelColliders[1].steerAngle = steerAngle;
            
            //Set steer angle for forks
            forkPivot.transform.localRotation = Quaternion.Euler(forkPivot.transform.localRotation.x, forkPivot.transform.localRotation.y + steerAngle, forkPivot.transform.localRotation.z);
        }

        void HandleLeanSpeed()
        {
            //lerps between min lean and max lean depending on the speed
            currentLeanTorque = Mathf.Lerp(minLeanTorque, maxLeanTorque, currentSpeed / speedForMaxLean);

        }

        void HandleEngine()
        {
            //Do I have to explain this?
            currentSpeed = rb.velocity.magnitude * 3.6f;

            //If an audio clip has been set
            if (sound)
            {
                //This Whole Functions determines the pitch of the bike sound and also the gear it should be in depending on the max speed
                //None of this should need to be changed
                float gearPercentage = (1 / (float)numGears);
                float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));

                currentGearPercent = Mathf.Lerp(currentGearPercent, targetGearFactor, Time.deltaTime * 5f);
                var gearNumFactor = currentGear / (float)numGears;

                //1 is top of the gear 0 is start
                rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPercent);

                float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
                float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
                float downGearMax = (1 / (float)numGears) * currentGear;

                //Sets Gear number for the UI
                if (currentGear > 0 && speedPercentage < downGearMax)
                {
                    currentGear--;
                }
                if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
                {
                    currentGear++;
                }

                //Pitch is changed
                float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
                pitch = Mathf.Clamp(pitch, 1, 3);

                if (wheelColliders[0].rpm>0.1f)
                {
                    reversing = false;
                }

                if (!reversing)
                {
                    sound.pitch = Mathf.Min(highPitch, pitch);
                }
                else
                {
                    sound.pitch = 1;
                }
            }
        }

        void SetWheelMeshPos()
        {
            //Set Wheel Position to wheel Collider position
            for (int i = 0; i < 2; i++)
            {
                //Vars for Get World Pose
                Quaternion quat;
                Vector3 position;

                //Assign wheel RPM
                wheelRPM = wheelColliders[i].rpm;

                //Gets Wheel Collider Possitions and rotations
                wheelColliders[i].GetWorldPose(out position, out quat);

                //Sets Wheels to collider positon
                wheels[i].transform.position = position;

                //Calculates Rotation speed of wheel collider so you can set the mesh rotation how you like
                if (rb.velocity.magnitude > 0)
                {
                    //Rotates
                    if (wheelRotationAxis == WheelRotationAxis.X)
                    {
                        wheels[i].transform.Rotate(Vector3.right * wheelRPM * 6 * Time.deltaTime);
                    }
                    if(wheelRotationAxis == WheelRotationAxis.Z)
                    {
                        wheels[i].transform.Rotate(Vector3.forward * wheelRPM * 6 * Time.deltaTime);
                    }
                    if(wheelRotationAxis == WheelRotationAxis.Y)
                    {
                        wheels[i].transform.Rotate(Vector3.up * wheelRPM * 6 * Time.deltaTime);
                    }
                }
            }
        }

        void CalculateTorque()
        {
            //Automatic Transmision Torque
            if (isAutomatic)
            {
                //If the bike has automatic transmission it will lerp between the max torque and min torque depending on the speed

                float torqueLerp = Mathf.Lerp(ogTorque, topGearTorque, currentSpeed/maxSpeed);

                //Wheel torque is set
                firstGearTorque = torqueLerp;
                //If Using Nitrous and Not using a forward force
                if (nitrousScript.isBoosting && !nitrousScript.forwardForce)
                {
                    //Increase Torque
                    firstGearTorque = firstGearTorque + nitrousScript.nitrousPower;
                }
            }

            //Manual Transmision Torque
            else
            {
                //If first gear set torque
                if (currentGear == 0)
                {
                    firstGearTorque = ogTorque;
                }

                //If last gear set torque
                else if (currentGear == numGears - 1)
                {
                    firstGearTorque = topGearTorque;
                }

                //Set the torque based on which gear you are in to simulate decrease in torque as you increase the gears
                else
                {
                    firstGearTorque = ogTorque - torqueDifference * currentGear;
                }
                //If Using Nitrous and Not using a forward force
                if (nitrousScript.isBoosting && !nitrousScript.forwardForce)
                {
                    //Increase Torque
                    firstGearTorque = firstGearTorque + nitrousScript.nitrousPower;
                }
            }
        }

        void AddTorque()
        {
            //Adds a forward force to The bikes
            if(nitrousScript.isBoosting && nitrousScript.forwardForce)
            {
                rb.AddForce(rb.transform.forward * nitrousScript.nitrousPower);
            }

            //remove wheel torque if maxSpeed
            if (currentSpeed > maxSpeed)
            {
                firstGearTorque = 0;
            }

            if (inputs.VInput > 0)
            {
                //Add torque to wheel
                wheelColliders[0].motorTorque = firstGearTorque * inputs.VInput;
            }

            //If your not leaning past the max lean angles
            if (rb.transform.eulerAngles.z < maxLeanLeft || rb.transform.eulerAngles.z > maxLeanRight && isGrounded)
            {
                if (inputs.LeanInput != -inputs.HzInput)
                {
                    //Set Lean Torque
                    rb.AddRelativeTorque(-Vector3.forward * inputs.LeanInput * currentLeanTorque, ForceMode.Acceleration);
                }

                //Set Lean Torque
                rb.AddRelativeTorque(-Vector3.forward * inputs.HzInput * steerLeanTorque, ForceMode.Acceleration);
            }

            if (!isGrounded)
            {
                //Lean In Air
                rb.AddRelativeTorque(-Vector3.forward * inputs.LeanInput * currentLeanTorque, ForceMode.Acceleration);
                //Rotate In Air
                rb.AddRelativeTorque(Vector3.up * inputs.HzInput * inAirSpinTorque);
                //BackFlip Torque
                rb.AddRelativeTorque(Vector3.right * inputs.WheelieInput * backFlipTorque);
            }
            else
            {
                //Wheelie Torque
                rb.AddRelativeTorque(-Vector3.right * -inputs.WheelieInput * wheelieTorque);
            }
        }

        void HandleBrakes()
        {
            //When space bar is pressed front brake torque is added else it is removed
            if (Input.GetKey(KeyCode.Space))
            {
                //Brake torque set
                wheelColliders[1].brakeTorque = frontBrakeTorque;
            }
            else
            {
                //Brake torque Removed
                wheelColliders[1].brakeTorque = 0;
            }
            
            //You can only brake if you are not to slow to only allow reversing when you are almost stationary
            if (inputs.VInput<0 && wheelColliders[0].rpm>0.1)
            {
                wheelColliders[0].motorTorque = 0;

                //Brake torque set
                wheelColliders[0].brakeTorque = backBrakeTorque;
            }
            else
            {
                //Brake torque Removed
                wheelColliders[0].brakeTorque = 0;

                //If not reversing too fast
                if (rb.velocity.magnitude < maxReverseSpeed)
                {
                    //Reverse
                    wheelColliders[0].motorTorque = inputs.VInput * firstGearTorque;
                    reversing = true;
                }
                else
                {
                    //Stop torque if too fast
                    wheelColliders[0].motorTorque = 0;
                }
            }
        }
        #endregion

    }
} 

