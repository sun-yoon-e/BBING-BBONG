using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public Camera FPSCam;
    public Camera TPSCam;
    public GameObject fullMap;
    public Image aim;
    public Rigidbody rb;

    public int nowCam;

    private void Start()
    {
        nowCam = 3;
        TPSCam.gameObject.SetActive(true);
        FPSCam.gameObject.SetActive(false);
        fullMap.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            if (nowCam == 1)
            {
                nowCam = 3;
                aim.gameObject.SetActive(false);
                TPSCam.gameObject.SetActive(true);
                FPSCam.gameObject.SetActive(false);
            }
            else
            {
                nowCam = 1;
                aim.gameObject.SetActive(true);
                TPSCam.gameObject.SetActive(false);
                FPSCam.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (fullMap.activeSelf == true)
                fullMap.SetActive(false);
            else if (fullMap.activeSelf == false)
                fullMap.SetActive(true);
        }
    }
}
