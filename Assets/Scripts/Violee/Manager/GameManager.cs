using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public enum EGameState
{
    Title,
    GeneratingMap,
    Playing,
    Winning,
}


public class GameManager : SingletonCS<GameManager>
{
    bool init;
    
    
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static string GameState => gameFsm.CurStateName;
    public static readonly BindDataState TitleState;
    public static void EnterTitle() => gameFsm.ChangeState(EGameState.Title);
    public static readonly BindDataState WinningState;
    public static void EnterWinning() => gameFsm.ChangeState(EGameState.Winning);
    public static readonly BindDataState GeneratingMapState;
    public static readonly BindDataState PlayingState;
    

    static GameManager()
    {
        TitleState = Binder.From(gameFsm.GetState(EGameState.Title));
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        WinningState = Binder.From(gameFsm.GetState(EGameState.Winning));
        
        MapManager.GenerateStream
            .Where(_ => IsTitle)
            .OnBegin(_ => gameFsm.ChangeState(EGameState.GeneratingMap));
        MapManager.DijkstraStream
            .OnEnd(param =>
            {
                PlayerMono.OnDijkstraEnd(BoxHelper.Pos2DTo3DPoint(param.StartPos, param.StartDir));
                MainItemMono.OnDijkstraEnd();
                gameFsm.ChangeState(EGameState.Playing);
            });
        Binder.Update(dt => gameFsm.Update(dt), EUpdatePri.Fsm);
        Binder.Update(_ =>
        {
            Time.timeScale = Configer.SettingsConfig.QuickKey && Input.GetKey(KeyCode.Q) ? 10f : 1f;
        }, EUpdatePri.Input);
    }
    
    public static bool IsTitle => gameFsm.IsState(EGameState.Title);
    public static bool IsPlaying => gameFsm.IsState(EGameState.Playing);
}
