using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class AudioMono : Singleton<AudioMono>
{
    public required AudioClip BGMKisekiIns;
    public required AudioClip BGMReturnsIns;
    static List<AudioClip> recordList = null!;
    static AudioSource titleBGMSource = null!;
    static AudioSource? curBGMSource;
    
    // List<AudioSource> seSource = [];
    // const int SeCount = 10;

    public static event Action<AudioClip>? OnUnPauseLoop;

    public static async Task Init() => await Instance._Init();
    async Task _Init()
    {
        try
        {
            recordList = await Resourcer.LoadAssetsAsyncByLabel<AudioClip>("VioleTAudio");
        
            titleBGMSource = gameObject.AddComponent<AudioSource>();
            // for (int i = 0; i < SeCount; i++)
            // {
            //     var go = new GameObject("SE_" + i);
            //     go.transform.SetParent(transform);
            //     seSource.Add(go.AddComponent<AudioSource>());
            // }

            // TODO BindState is deleted
            // GameState.TitleState.OnEnter(() =>
            // {
            //     lastestClips.Add(BGMKisekiIns);
            //     PlayLoop(titleBGMSource, () => BGMKisekiIns);
            // });
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }

    [ShowInInspector]
    static readonly HashSet<AudioClip> lastestClips = [];
    public static AudioClip GetRandomClips()
    {
        if (lastestClips.Count == recordList.Count)
        {
            lastestClips.Clear();
        }
        var newClip = recordList.RandomItem(filter: x => x != CurClip && !lastestClips.Contains(x)) ?? Instance.BGMKisekiIns;
        lastestClips.Add(newClip);
        return newClip;
    }
    public static void PlayWinLoop()
    {
        PlayLoop(titleBGMSource, () => Instance.BGMReturnsIns);
    }
    
    public static void PlayLoop(AudioSource audioSource, Func<AudioClip> getClip, float volume = 0.4f, bool mute = false, bool loop = true)
    {
        if(curBGMSource != null)
            curBGMSource.Pause();
        // audioSource.getClip = recordList.RandomItem();
        audioSource.clip = getClip();
        audioSource.volume = volume;
        audioSource.mute = mute;
        audioSource.loop = loop;
        audioSource.Play();
        curBGMSource = audioSource;
        UnPauseLoop();
    }
    
    public static AudioClip? CurClip => curBGMSource?.clip;

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
    
    // [Button]
    // public void PlayTest()
    // {
    //     PlayLoop(titleBGMSource, Instance.BGMKisekiIns);
    // }
    //
    // [Button]
    // public void PlayTestRandom()
    // {
    //     PlayLoop(titleBGMSource, recordList.RandomItem());
    // }
}