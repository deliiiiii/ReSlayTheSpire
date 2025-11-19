using System;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;

public static class FSM
{
    public static readonly GameData GameData = new();
}

[Serializable]
public class GameData: FSMArg<GameData, EGameState>
{
    public string PlayerName;
    public bool HasLastBuff;

    BattleData battleData;
    protected override void Bind()
    {
        GetState(EGameState.Battle)
            .OnEnter(() =>
            {
                battleData = new BattleData(this);
                battleData.Launch(EBattleState.SelectLastBuff);
            })
            .OnExit(() =>
            {
                battleData.Release();
                battleData = null!;
            });
    }

    protected override void Launch()
    {
    }

    protected override void UnInit()
    {
    }
}