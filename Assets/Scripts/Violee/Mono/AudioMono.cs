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

    public static AudioClip BGMKiseki => Instance.BGMKisekiIns;
    public static List<AudioClip> BGMRecordPlayer => Instance.BGMRecordPlayerIns;
    public static AudioClip BGMVioletLine => Instance.BGMVioletLineIns;

    AudioSource titleBGMSource = null!;
    static AudioSource? curBGMSource;
    
    List<AudioSource> seSource = [];
    const int SeCount = 10;

    public static event Action<AudioClip>? OnUnPauseLoop;

    protected override void Awake()
    {
        base.Awake();
        titleBGMSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < SeCount; i++)
        {
            var go = new GameObject("SE_" + i);
            go.transform.SetParent(transform);
            seSource.Add(go.AddComponent<AudioSource>());
        } 
    }


    public static void PlayLoop(AudioSource audioSource, AudioClip clip, bool mute = false, bool loop = true)
    {
        curBGMSource?.Pause();
        curBGMSource = audioSource;
        // audioSource.clip = BGMRecordPlayer.RandomItem();
        curBGMSource.clip = clip;
        curBGMSource.mute = mute;
        curBGMSource.loop = loop;
        curBGMSource.Play();
        UnPauseLoop();
    }

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
        PlayLoop(titleBGMSource, BGMKiseki);
    }
    
    [Button]
    public void PlayTestRandom()
    {
        PlayLoop(titleBGMSource, BGMRecordPlayer.RandomItem());
    }
}