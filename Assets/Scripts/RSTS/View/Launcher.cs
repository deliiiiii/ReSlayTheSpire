using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    public List<ViewBase> ViewList = [];
    // ReSharper disable once Unity.IncorrectMethodSignature
    // ReSharper disable once UnusedMember.Local
    async UniTask Start()
    {
        try
        {
            await Loader.LoadAll();
            ViewList.ForEach(v => v.Bind());
            GameFSM.Register(EGameState.Title, new GameData());
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
        }
    }

    // void OnExit()
    // {
    //     GameFSM.Release();
    // }

    // void Update()
    // {
    //     // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    // }
}