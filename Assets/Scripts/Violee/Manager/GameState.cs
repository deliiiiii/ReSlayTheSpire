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


public class GameState : SingletonCS<GameState>
{
    static readonly MyFSM<EGameState> gameFsm = new ();
    public static readonly BindDataState TitleState;
    public static void EnterTitle() => gameFsm.ChangeState(EGameState.Title);
    public static readonly BindDataState WinningState;
    public static void EnterWinning() => gameFsm.ChangeState(EGameState.Winning);
    public static readonly BindDataState GeneratingMapState;
    public static void EnterGeneratingMap() => gameFsm.ChangeState(EGameState.GeneratingMap);
    public static readonly BindDataState PlayingState;
    public static void EnterPlaying() => gameFsm.ChangeState(EGameState.Playing);
    

    static GameState()
    {
        TitleState = Binder.From(gameFsm.GetState(EGameState.Title));
        GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        WinningState = Binder.From(gameFsm.GetState(EGameState.Winning));
        
        Binder.Update(dt => gameFsm.Update(dt), EUpdatePri.Fsm);
        Binder.Update(_ =>
        {
            Time.timeScale = Configer.SettingsConfig.QuickKey && Input.GetKey(KeyCode.Q) ? 10f : 1f;
        }, EUpdatePri.Input);
    }
    
    public static bool IsTitle => gameFsm.IsOneOfState(EGameState.Title);
    public static bool IsPlaying => gameFsm.IsOneOfState(EGameState.Playing);
}
