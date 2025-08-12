using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class AudioMono : Singleton<AudioMono>
{
    // ReSharper disable once IdentifierTypo
    public required AudioClip BGMKisekiIns;
    public required List<AudioClip> BGMRecordPlayerIns;
    public required AudioClip BGMVioletLineIns;
    public required AudioClip BGMReturnsIns;

    public static List<AudioClip> BGMRecordPlayer => Instance.BGMRecordPlayerIns;

    static AudioSource titleBGMSource = null!;
    static AudioSource? curBGMSource;
    
    List<AudioSource> seSource = [];
    const int SeCount = 10;

    public static event Action<AudioClip>? OnUnPauseLoop;

    public static void Init() => Instance._Init();
    void _Init()
    {
        base.Awake();
        titleBGMSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < SeCount; i++)
        {
            var go = new GameObject("SE_" + i);
            go.transform.SetParent(transform);
            seSource.Add(go.AddComponent<AudioSource>());
        }

        GameManager.TitleState.OnEnter(() =>
        {
            PlayLoop(titleBGMSource, BGMKisekiIns);
        });

    }


    public static void PlayWinLoop()
    {
        PlayLoop(titleBGMSource, Instance.BGMReturnsIns);
    }
    public static void PlayLoop(AudioSource audioSource, AudioClip clip, float volume = 0.4f, bool mute = false, bool loop = true)
    {
        if(curBGMSource != null)
            curBGMSource.Pause();
        curBGMSource = audioSource;
        // audioSource.clip = BGMRecordPlayer.RandomItem();
        curBGMSource.clip = clip;
        curBGMSource.volume = volume;
        curBGMSource.mute = mute;
        curBGMSource.loop = loop;
        curBGMSource.Play();
        UnPauseLoop();
    }
    
    public static AudioClip? CurClip => curBGMSource!.clip;

    static void PauseLoop()
    {
        curBGMSource?.Pause();
    }
    
    static void UnPauseLoop()
    {
        if (curBGMSource == null)
            return;
        curBGMSource.UnPause();
        OnUnPauseLoop?.Invoke(curBGMSource.clip);
    }
    
    [Button]
    public void PlayTest()
    {
        PlayLoop(titleBGMSource, Instance.BGMKisekiIns);
    }
    
    [Button]
    public void PlayTestRandom()
    {
        PlayLoop(titleBGMSource, BGMRecordPlayer.RandomItem());
    }
}