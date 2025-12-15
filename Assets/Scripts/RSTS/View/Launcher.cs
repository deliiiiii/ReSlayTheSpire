using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RSTS.Game;
public class Launcher : Singleton<Launcher>
{
    public List<ViewBase> ViewList = [];
    [SerializeField]
    public GameFSM GameFSM = null!;
    // ReSharper disable once Unity.IncorrectMethodSignature
    // ReSharper disable once UnusedMember.Local
    async UniTask Start()
    {
        try
        {
            await Loader.LoadAll();
            ViewList.ForEach(v => v.Bind());
            GameFSM = new();
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
        }
    }
    
    // void Update()
    // {
    //     // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    // }
}