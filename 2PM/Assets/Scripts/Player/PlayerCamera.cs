using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera FPSCam;
    public Camera TPSCam;
    public Camera AICam;
    public GameObject fullMap;
    public Image aim;
    public Rigidbody rb;

    public int nowCam;

    private void Start()
    {
        instance = this;

        if (GameClient.Instance.client_host)
            AICam = GameObject.Find("AIPlayer(Clone)").transform.Find("AICamera").GetComponent<Camera>();
        
        nowCam = 3;
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
                FPSCam.gameObject.SetActive(false);
                if (GameClient.Instance.client_host)
                    AICam.gameObject.SetActive(false);
                TPSCam.gameObject.SetActive(true);
            }
            else
            {
                nowCam = 1;
                aim.gameObject.SetActive(false);
                TPSCam.gameObject.SetActive(false);
                if (GameClient.Instance.client_host)
                    AICam.gameObject.SetActive(false);
                FPSCam.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            nowCam = 2;
            aim.gameObject.SetActive(false);
            TPSCam.gameObject.SetActive(false);
            FPSCam.gameObject.SetActive(false);
            if (GameClient.Instance.client_host)
                AICam.gameObject.SetActive(true);
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