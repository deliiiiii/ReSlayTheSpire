using System;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;


[Serializable]
public class GameData: IMyFSMArg<GameFSM>
{
    public string PlayerName;
    public bool HasLastBuff;

    public void Init() { }
    public void Bind(GameFSM fsm)
    {
        fsm.GetState(EGameState.Battle)
            .OnEnter(FSM.Game.Battle.Register)
            .OnExit(FSM.Game.Battle.Release);
    }

    public void Bind(Func<EGameState, MyState> getState)
    {
       
    }

    public void Launch()
    {
    }

    public void UnInit()
    {
    }
}