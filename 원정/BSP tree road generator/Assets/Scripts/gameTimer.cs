using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class gameTimer : MonoBehaviour
{
    public Text timerText;
    private bool timerActive = false;
    private float limitTime = 60 * 3;       //제한시간 600초(10분)
    private float currentTime;
    
    
    public void StartTimer()
    {
        currentTime = limitTime;
        timerActive = true;
        timerText.gameObject.SetActive(true);
    }

    public void stopTimer()
    {
        timerActive = false;
        timerText.gameObject.SetActive(false);
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
