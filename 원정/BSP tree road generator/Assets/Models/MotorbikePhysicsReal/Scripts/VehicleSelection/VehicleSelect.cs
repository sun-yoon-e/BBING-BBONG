using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadd420;
public class VehicleSelect : MonoBehaviour
{
    public GameObject motocross;
    public GameObject moped;
    public GameObject chopper;
    public GameObject bicycle;

    public ThirdPersonCamera cameraScript;
    public KeyBoardShortCuts shortCutScript;

    public GameObject canvas;

    public NitrousUI nitrousUI;


    //These functions are for the Buttons when you play the test scene

    public void SelectMotocross()
    {
        cameraScript.lookAt = motocross.transform;
        shortCutScript.currentBike = motocross.transform;
        nitrousUI.nitrousScript = motocross.gameObject.GetComponent<NitrousManager>();
        moped.SetActive(false);
        chopper.SetActive(false);
        bicycle.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void SelectMoped()
    {
        cameraScript.lookAt = moped.transform;
        shortCutScript.currentBike = moped.transform;
        nitrousUI.nitrousScript = moped.gameObject.GetComponent<NitrousManager>();
        motocross.SetActive(false);
        chopper.SetActive(false);
        bicycle.SetActive(false);
        canvas.SetActive(false);
    }

    public void SelectChopper()
    {
        cameraScript.lookAt = chopper.transform;
        shortCutScript.currentBike = chopper.transform;
        nitrousUI.nitrousScript = chopper.gameObject.GetComponent<NitrousManager>();
        motocross.SetActive(false);
        moped.SetActive(false);
        bicycle.SetActive(false);
        canvas.SetActive(false);
    }

    public void SelectBicycle()
    {
        cameraScript.lookAt = bicycle.transform;
        shortCutScript.currentBike = bicycle.transform;
        nitrousUI.nitrousScript = bicycle.gameObject.GetComponent<NitrousManager>();
        motocross.SetActive(false);
        moped.SetActive(false);
        chopper.SetActive(false);
        canvas.SetActive(false);
    }
}
