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
            Debug.Log("부스터");
            boosterTimer += Time.deltaTime;
            if (boosterTimer >= 5f)
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
            nitrousSFX.Play();
            isBoosting = true;
        }
        else
        {
            vfx.SetActive(false);
            nitrousSFX.Stop();
        }
    }
}
