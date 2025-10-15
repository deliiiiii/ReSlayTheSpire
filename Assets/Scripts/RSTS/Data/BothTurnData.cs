using System;
using System.Collections.Generic;
using RSTS.CDMV;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;

[Serializable]
public class BothTurnData : IRefSimple
{
    public int PlayerDefend;
    public int TurnID;
    public List<EnemyData> EnemyList = [];

    public void EnterBothTurn(/* TODO */)
    {
        PlayerDefend = TurnID = 0;
    }
}

[Serializable]
public class EnemyData
{
    public EnemyConfigMulti Config;
}