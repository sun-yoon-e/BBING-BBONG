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
    public static SoundManager instance;
    [Header("사운드 등록")]
    [SerializeField] private Sound[] bgmSounds;
    [SerializeField] private Sound[] sfxSounds;

    [Header("BGM플레이어")]
    [SerializeField] public AudioSource bgmPlayer;
    [Header("SFX플레이어")]
    [SerializeField] public AudioSource[] sfxPlayer;

    public int bgmNum;
    public bool reload;

    private void Start()
    {
        instance = this;
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

    public void PlaySE(string _soundName)
    {
        for (int i = 0; i < sfxSounds.Length; i++)
        {
            if (_soundName == sfxSounds[i].soundName)
            {
                for (int j = 0; j < sfxPlayer.Length; j++)
                {
                    if (!sfxPlayer[j].isPlaying)
                    {
                        sfxPlayer[j].clip = sfxSounds[i].clip;
                        sfxPlayer[j].Play();
                        return;
                    }
                }

                return;
            }
        }
    }
}
