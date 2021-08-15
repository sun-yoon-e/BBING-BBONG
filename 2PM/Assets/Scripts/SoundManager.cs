using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    [Header("사운드 등록")]
    [SerializeField] private Sound[] bgmSounds;
    [SerializeField] private Sound[] effectSounds;

    [Header("BGM플레이어")]
    [SerializeField] public AudioSource bgmPlayer;

    public int bgmNum;
    public bool reload;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        reload = false;
        bgmNum = 0;
        PlayeBGM();
    }

    public void PlayeBGM()
    {
        bgmPlayer.clip = bgmSounds[bgmNum].clip;
        bgmPlayer.Play();
    }
}
