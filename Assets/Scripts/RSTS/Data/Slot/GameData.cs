using System;
using RSTS.CDMV;
using UnityEngine;


#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public enum EPlayerJob
{
    ZhanShi,
    LieShou,
    JiBao,
    GuanZhe
}



[Serializable]
public class GameData: IMyFSMArg
{
    public string PlayerName;
    public bool HasLastBuff;

    [SerializeReference] public BattleData BattleData;
    public BattleData CreateBattleData(EPlayerJob job) => BattleData = new BattleData(this, job);
    public void Launch()
    {
    }

    public void UnInit()
    {
    }

    public void Save() => Saver.Save("DataSaved", nameof(GameData), this);

    public GameData Load() => Saver.Load<GameData>("DataSaved", nameof(GameData));
}

