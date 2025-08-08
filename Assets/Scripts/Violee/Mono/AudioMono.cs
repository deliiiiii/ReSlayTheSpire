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

    AudioSource bgmSource = null!;
    List<AudioSource> seSource = [];

    const int SeCount = 10;

    public static event Action<AudioClip>? OnPlayLoop;

    protected override void Awake()
    {
        base.Awake();
        bgmSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < SeCount; i++)
        {
            var go = new GameObject("SE_" + i);
            go.transform.SetParent(transform);
            seSource.Add(go.AddComponent<AudioSource>());
        } 
    }


    public void PlayLoop(AudioClip clip, bool mute = false, bool loop = true)
    {
        bgmSource.clip = BGMRecordPlayer.RandomItem();
        bgmSource.mute = mute;
        bgmSource.loop = loop;
        bgmSource.Play();
        OnPlayLoop?.Invoke(clip);
    }

    [Button]
    public void PlayTest()
    {
        PlayLoop(BGMKiseki);
    }
}