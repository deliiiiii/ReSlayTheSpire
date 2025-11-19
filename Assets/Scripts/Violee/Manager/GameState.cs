using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
#pragma warning disable CS0612 // 类型或成员已过时

namespace Violee;

public enum EGameState
{
    Title,
    GeneratingMap,
    Playing,
    Winning,
}

public class GameData : IMyFSMArg<GameFSM>
{
    public void Init() { }
    public void Bind(GameFSM fsm)
    {
    }

    public void Bind(Func<EGameState, MyState> getState) { }
    public void Launch() { }
    public void UnInit() { }
}

public class GameFSM : MyFSMForData<GameFSM, EGameState, GameData>
{
    public override string SavePreName => "VioleeSaved";
    public override string SaveFileName => nameof(GameFSM);
}

public class GameState : SingletonCS<GameState>
{
    static readonly GameFSM gameFsm = new ();
    // TODO BindState is deleted
    // public static readonly BindState TitleState;
    public static void EnterTitle() => gameFsm.EnterState(EGameState.Title);
    // TODO BindState is deleted
    // public static readonly BindState WinningState;
    public static void EnterWinning() => gameFsm.EnterState(EGameState.Winning);
    // TODO BindState is deleted
    // public static readonly BindState GeneratingMapState;
    public static void EnterGeneratingMap() => gameFsm.EnterState(EGameState.GeneratingMap);
    // TODO BindState is deleted
    // public static readonly BindState PlayingState;
    public static void EnterPlaying() => gameFsm.EnterState(EGameState.Playing);
    

    static GameState()
    {
        // TODO BindState is deleted
        // TitleState = Binder.From(gameFsm.GetState(EGameState.Title));
        // GeneratingMapState = Binder.From(gameFsm.GetState(EGameState.GeneratingMap));
        // PlayingState = Binder.From(gameFsm.GetState(EGameState.Playing));
        // WinningState = Binder.From(gameFsm.GetState(EGameState.Winning));
        
        // TODO Binder.Update is obsolete
        // Binder.Update(dt => gameFsm.Update(dt), EUpdatePri.Fsm);
        // Binder.Update(_ =>
        // {
        //     Time.timeScale = Configer.SettingsConfig.QuickKey && Input.GetKey(KeyCode.Q) ? 10f : 1f;
        // }, EUpdatePri.Input);
    }
    
    // public static bool IsTitle => gameFsm.IsOneOfState(EGameState.Title);
    // public static bool IsPlaying => gameFsm.IsOneOfState(EGameState.Playing);
}
