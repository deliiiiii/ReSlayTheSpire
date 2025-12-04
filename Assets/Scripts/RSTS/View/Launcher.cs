using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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
            new GameData().Launch(EGameState.Title);
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