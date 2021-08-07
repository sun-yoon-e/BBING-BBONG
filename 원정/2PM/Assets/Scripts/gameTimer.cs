using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class gameTimer : MonoBehaviour
{
    public Text timerText;
    public bool timerActive = true;
    public float limitTime = 600;       //제한시간 600초(10분)
    private float currentTime;
    
    
    void Start()
    {
        currentTime = limitTime;
    }
    
    void Update()
    {
        if (timerActive == true)
        {
            currentTime = currentTime - Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
    }
}
