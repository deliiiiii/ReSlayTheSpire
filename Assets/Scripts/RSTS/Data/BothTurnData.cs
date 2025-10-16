using System;
using System.Collections.Generic;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;

[Serializable]
public class BothTurnData
{
    public int PlayerDefend;
    public int TurnID;
    public List<EnemyDataBase> EnemyList = [];

    public void EnterBothTurn()
    {
        PlayerDefend = TurnID = 0;
        EnemyList.Clear();
        EnemyList.Add(EnemyDataBase.CreateEnemy(0));
        EnemyList.Add(EnemyDataBase.CreateEnemy(1));
        EnemyList.Add(EnemyDataBase.CreateEnemy(0));
    }
    
}


