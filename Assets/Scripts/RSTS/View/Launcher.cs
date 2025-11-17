using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RSTS.CDMV;

namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    async UniTask Start()
    {
        try
        {
            await Loader.LoadAll();
            MyFSM.Register(GameStateWrap.One, EGameState.Title, _ => new GameData());
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
        }
    }

    void OnExit()
    {
        MyFSM.Release(GameStateWrap.One);
    }

    // void Update()
    // {
    //     // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    // }
}