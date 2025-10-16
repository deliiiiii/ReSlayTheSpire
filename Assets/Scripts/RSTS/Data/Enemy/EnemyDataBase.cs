using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;
[Serializable]
public abstract class EnemyDataBase
{
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
                var ins = (Activator.CreateInstance(type) as EnemyDataBase)!;
                ins.ReadConfig(config);
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
    
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public EnemyConfigMulti Config;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public Observable<int> CurHP = new(0);

    void ReadConfig(EnemyConfigMulti config)
    {
        Config = config;
        CurHP.Value = Config.MaxHP;
    }
}
[AttributeUsage(AttributeTargets.Class)]
public class EnemyIDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}