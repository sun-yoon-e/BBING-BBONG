using Gadd420;
using UnityEngine;

public class NitrousManager : MonoBehaviour
{
    [HideInInspector]public bool isBoosting;
    [Header("Either Torque or Force Depending on Forward Force")]
    public float nitrousPower = 200;
    [Header("Nitrous Adds Torque if this is false")]
    public bool forwardForce;
    [Header("Time in Seconds")]
    public float timeToEmptyTank = 3;
    public float timeToRefil =  5;
    public int numberOfTanks = 3;
    
    [Header("So you're not forced to use all the nitrous at once")]
    public bool saveNitrous = true;

    [Header("Effects")]
    public GameObject vfx;
    public AudioSource nitrousSFX;

    //Not For Inspector
    bool isHoldingNitrous;
    float ogTimeToRefilTanks;
    [HideInInspector]public float ogTimeToEmptyTanks;
    float ogNoTanks;
    bool startBoost;
    public bool hasNitrous = true;

    // Start is called before the first frame update
    void Start()
    {
        if (hasNitrous)
        {
            //Set for references to the origional values
            ogTimeToRefilTanks = timeToRefil;
            ogTimeToEmptyTanks = timeToEmptyTank;
            ogNoTanks = numberOfTanks;

            vfx.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (hasNitrous)
        {
            //Change this to what you want or put an axis in the input manager
            isHoldingNitrous = Input.GetKey(KeyCode.LeftAlt);
            if (isHoldingNitrous)
            {
                //Sets off the nitrous when saveNitrous is turned off
                startBoost = true;
            }

            //Cancels the SFX and VFX when out of Nos
            if (isHoldingNitrous && numberOfTanks == 0)
            {
                nitrousSFX.Stop();
                vfx.SetActive(false);
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (hasNitrous)
        {
            if (saveNitrous)
            {
                HoldNitrous();
            }
            else
            {
                BoostNitrous();
            }
        }
        
    }

    void HoldNitrous()
    {
        if (isHoldingNitrous)
        {
            //If there fuel
            if(timeToEmptyTank>0 && numberOfTanks > 0)
            {
                //Turns on Effects
                vfx.SetActive(true);
                nitrousSFX.Play();
                isBoosting = true;//fixed update runs 50 times a second so minusing 0.02 will make it minus 1 every second    
                timeToEmptyTank = timeToEmptyTank - 0.02f;
                //minuses a different amount to time to refil so they reach 0 at the same time
                timeToRefil = timeToRefil - (0.02f * (ogTimeToRefilTanks / ogTimeToEmptyTanks));


            }
            else
            {
                //Stops Effects
                vfx.SetActive(false);
                nitrousSFX.Stop();
            }
            //if fuel is empty but theres another tank -1 from the rank and reset the time to empty tank and time to refil
            if (timeToEmptyTank <= 0 && numberOfTanks > 0)
            {
                numberOfTanks--;
                timeToRefil = ogTimeToRefilTanks;
                timeToEmptyTank = ogTimeToEmptyTanks;
            }
        }
        else
        {
            NitrousRefil();
        }

        //Makes sure numbers stay between 0  and og time;
        timeToRefil = Mathf.Clamp(timeToRefil,0, ogTimeToRefilTanks);
        timeToEmptyTank = Mathf.Clamp(timeToEmptyTank, 0, ogTimeToEmptyTanks);
    }

    void BoostNitrous()
    {
        if (startBoost)
        {
            if (timeToEmptyTank > 0 && numberOfTanks > 0)
            {
                vfx.SetActive(true);
                nitrousSFX.Play();
                isBoosting = true;
                timeToEmptyTank = timeToEmptyTank - 0.02f;
                
                timeToRefil = timeToRefil - (0.02f * (ogTimeToRefilTanks / ogTimeToEmptyTanks));

            }
            else
            {
                vfx.SetActive(false);
                nitrousSFX.Stop();
            }
            if (timeToEmptyTank <= 0 && numberOfTanks > 0)
            {
                numberOfTanks--;
                timeToRefil = ogTimeToRefilTanks;
                timeToEmptyTank = ogTimeToEmptyTanks;
                //makes you have to press the button again to use the next tank
                startBoost = false;
            }
            if(numberOfTanks == 0)
            {
                numberOfTanks = 1;
                timeToEmptyTank = 0;
                timeToRefil = 0;
            }
        }
        else
        {
            NitrousRefil();
        }

        timeToRefil = Mathf.Clamp(timeToRefil, 0, ogTimeToRefilTanks);
        timeToEmptyTank = Mathf.Clamp(timeToEmptyTank, 0, ogTimeToEmptyTanks);
    }
    
    void NitrousRefil()
    {
        vfx.SetActive(false);
        nitrousSFX.Stop();
        isBoosting = false;

        //adds to the timer
        if (timeToRefil < ogTimeToRefilTanks)
        {
            timeToRefil = timeToRefil + 0.02f;
        }

        //This makes the timeToEmptyTank refuel based on the timeToRefil

        //Example 2 seconds to empty and 20 seconds to refil
        //when time to refil is 10 seconds left time to empty will be 1 second
        float inverseLerp = Mathf.InverseLerp(0, ogTimeToRefilTanks, timeToRefil);
        float lerp = Mathf.Lerp(0, ogTimeToEmptyTanks, inverseLerp);

        timeToEmptyTank = lerp;

        //refils tanks
        if (timeToRefil == ogTimeToRefilTanks && numberOfTanks != ogNoTanks)
        {
            numberOfTanks++;
            timeToRefil = 0;
            timeToEmptyTank = 0;
        }
    }

}
