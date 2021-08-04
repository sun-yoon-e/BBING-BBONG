using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gadd420;


public class NitrousUI : MonoBehaviour
{

    public NitrousManager nitrousScript;
    VehicleSelect vehicleSelect;
    public Text tankNo;
    public Slider slider;
    [Header("False If you arent using my vehicle Selection script")]
    public bool inTestScene;

    // Start is called before the first frame update
    void Start()
    {
        if (inTestScene)
        {
            vehicleSelect = GetComponentInParent<VehicleSelect>();
            slider = GetComponentInChildren<Slider>();
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (nitrousScript)
        {
            slider.maxValue = nitrousScript.ogTimeToEmptyTanks;
            tankNo.text = (nitrousScript.numberOfTanks + "x");
            slider.value = nitrousScript.timeToEmptyTank;
        }

        
    }
}
