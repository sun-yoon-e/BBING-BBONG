using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public Camera FPSCam;
    public Camera TPSCam;

    private int nowCam;

    private void Start()
    {
        nowCam = 3;
        TPSCam.gameObject.SetActive(true);
        FPSCam.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            if (nowCam == 1)
            {
                nowCam = 3;
                TPSCam.gameObject.SetActive(true);
                FPSCam.gameObject.SetActive(false);
            }
            else
            {
                nowCam = 1;
                TPSCam.gameObject.SetActive(false);
                FPSCam.gameObject.SetActive(true);
            }
        }
    }
}
