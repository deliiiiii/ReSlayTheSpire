using System;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;


[Serializable]
public class GameData: IMyFSMArg<EGameState>
{
    public string PlayerName;
    public bool HasLastBuff;

    public void Bind(Func<EGameState, MyState> getState)
    {
        getState(EGameState.Battle)
            .OnEnter(() =>
            {
                FSM.Game.Battle.Register(EBattleState.SelectLastBuff, new BattleData());
            })
            .OnExit(FSM.Game.Battle.Release);
    }

    public void Launch()
    {
    }

    public void UnInit()
    {
    }
}