using Gadd420;
using UnityEngine;

public class NitrousManager : MonoBehaviour
{
    [Header("Either Torque or Force Depending on Forward Force")]
    public float nitrousPower = 200;
    [Header("Nitrous Adds Torque if this is false")]
    public bool forwardForce;

    [Header("Effects")]
    public GameObject vfx;
    public AudioSource nitrousSFX;

    //Not For Inspector
    public bool isBoosting;
    private float boosterTimer;
    public float boostTime;

    // Start is called before the first frame update
    void Start()
    {
        vfx.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isBoosting)
        {
            boosterTimer += Time.deltaTime;
            if (boosterTimer >= boostTime)
            {
                isBoosting = false;
                boosterTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        BoostNitrous();
    }

    void BoostNitrous()
    {
        if (isBoosting)
        {
            vfx.SetActive(true);
            isBoosting = true;
        }
        else
        {
            vfx.SetActive(false);
        }
    }
}