using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class gameTimer : MonoBehaviour
{
    public Text timerText;
    public Text countText;
    public bool isStart;
    
    private bool timerActive = false;
    private float gameTime;
    private float limitTime = 100;//60 * 1;       //제한시간 600초(10분)

    public static gameTimer instance;
    
    private void Start()
    {
        instance = this;
        
        timerText.gameObject.SetActive(false);
        StartCoroutine (StartCount());
    }

    IEnumerator StartCount()
    {
        yield return new WaitForSecondsRealtime(1);
        SoundManager.instance.PlaySE("Three");
        countText.text = "3";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SoundManager.instance.PlaySE("Two");
        countText.gameObject.SetActive(false);
        countText.text = "2";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SoundManager.instance.PlaySE("One");
        countText.gameObject.SetActive(false);
        countText.text = "1";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        SoundManager.instance.PlaySE("Go");
        countText.gameObject.SetActive(false);
        countText.text = "Go!";
        countText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        countText.gameObject.SetActive(false);

        isStart = true;
        StartTimer();
    }
    
    public void StartTimer()
    {
        gameTime = limitTime;
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
            gameTime = gameTime - Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(gameTime);
            timerText.text = $"{time.Minutes:00}:{time.Seconds:00}";//.{time.Milliseconds:000}";

            //if (time.Seconds == 0) stopTimer();
        }
    }
}
