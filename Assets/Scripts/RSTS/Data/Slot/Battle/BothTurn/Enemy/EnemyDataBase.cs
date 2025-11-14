using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;
[Serializable]
public abstract class EnemyDataBase
{
    #region Factory
    static Dictionary<int, Func<EnemyDataBase>> enemyDic = [];
    
    public static void InitEnemyDic()
    {
        enemyDic.Clear();
        var subTypeDic = typeof(EnemyDataBase).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(EnemyDataBase))
                        && x.GetAttribute<EnemyIDAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<EnemyIDAttribute>().ID);
        foreach (var config in RefPoolMulti<EnemyConfigMulti>.Acquire())
        {
            if (!subTypeDic.TryGetValue(config.ID, out var type))
            {
                MyDebug.LogError($"class Enemy{config.ID} not found");
                continue;
            }
            enemyDic.Add(config.ID, () =>
            {
                var ins = (Activator.CreateInstance(type, args: config) as EnemyDataBase)!;
                return ins;
            });
        }
    }
    public static EnemyDataBase CreateEnemy(int id)
    {
        if (enemyDic.TryGetValue(id, out var func))
        {
            return func();
        }
        throw new Exception($"Enemy ID {id} out of range");
    }
    #endregion

    protected EnemyDataBase(EnemyConfigMulti config)
    {
        Config = config;
        HPAndBuffData.MaxHP.Value = Config.MaxHP;
        HPAndBuffData.CurHP.Value = Config.MaxHP;
    }

    public EnemyConfigMulti Config;
    public HPAndBuffData HPAndBuffData = new();
    
    public UniTask DoCurIntention(BothTurnData bothTurnData, out List<AttackResultBase> resultList)
    {
        IntentionEnumerator ??= IntentionSeq().GetEnumerator();
        resultList = [];
        if (!IntentionEnumerator.MoveNext())
            return UniTask.CompletedTask;
        switch (IntentionEnumerator.Current)
        {
            case IntentionAttack attack:
                bothTurnData.AttackPlayerFromEnemy(this, attack.Attack, out resultList);
                return UniTask.CompletedTask;
            case IntentionBlock block:
                HPAndBuffData.Block.Value += block.Block;
                return UniTask.CompletedTask;
            case IntentionDebug debug:
                MyDebug.Log(debug.Msg);
                return UniTask.CompletedTask;
            default:
                return UniTask.CompletedTask;
        }
    }

    protected IEnumerator<IntentionBase>? IntentionEnumerator;
    protected abstract IEnumerable<IntentionBase> IntentionSeq();
    protected IntentionBase NthIntention(int n)
    {
        if (n < 0 || n >= Config.IntentionList.Count)
        {
            MyDebug.LogError($"{Config.name}'s intention[{n}] out of range");
            return IntentionNone.One;
        }
        return Config.IntentionList[n];
    }
}
[AttributeUsage(AttributeTargets.Class)]
public class EnemyIDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}