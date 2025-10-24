using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;

namespace RSTS;

[Serializable]
public abstract class CardDataBase
{
    static Dictionary<int, Func<CardDataBase>> cardDic = [];
    public static void InitCardDic()
    {
        cardDic.Clear();
        var subTypeDic = typeof(CardDataBase).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(CardDataBase))
                        && x.GetAttribute<CardIDAttribute>() != null)
            .ToDictionary(x => x.GetAttribute<CardIDAttribute>().ID);
        foreach (var config in RefPoolMulti<CardConfigMulti>.Acquire())
        {
            if (!subTypeDic.TryGetValue(config.ID, out var type))
            {
                MyDebug.LogError($"class Card{config.ID} not found");
                cardDic.Add(config.ID, () => new Card_Template{ Config = config });
                continue;
            }
            cardDic.Add(config.ID, () =>
            {
                var ins = (Activator.CreateInstance(type) as CardDataBase)!;
                ins.Config = config;
                ins.OnCreate();
                return ins;
            });
        }
    }
    public static CardDataBase CreateCard(int id)
    {
        if (cardDic.TryGetValue(id, out var func))
        {
            return func();
        }
        throw new Exception($"Card ID {id} out of range");
    }
    
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public CardConfigMulti Config;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
    public int UpgradeLevel;
    public List<CardComponentBase> ComponentList = [];
    
    public bool HasComponent<T>() where T : CardComponentBase, new()
        => ComponentList.OfType<T>().FirstOrDefault() is not null;
    public T GetComponent<T>() where T : CardComponentBase, new()
    {
        if (!HasComponent<T>(out var component))
            throw new Exception($"CardDataBase GetComponent<{typeof(T).Name}> not found");
        return component;
    }
    public abstract void OnCreate();
    public abstract void Yield(BothTurnData bothTurnData);
    public bool CanUpgrade => UpgradeLevel < Config.Upgrades.Count - 1;
    public bool ContainsKeyword(ECardKeyword keyword) => CurUpgradeInfo.Keywords.Contains(keyword);
    public CardCostBase CurCostInfo => CurUpgradeInfo.CostInfo;
    public EmbedString CurDes => CurUpgradeInfo.Des;
    
    protected int EmbedInt(int id) => CurUpgradeInfo.Des.EmbedIntList[id];
    protected bool HasComponent<T>(out T component) where T : CardComponentBase, new()
        => (component = ComponentList.OfType<T>().FirstOrDefault()!) is not null;
    protected T AddComponent<T>() where T : CardComponentBase, new()
    {
        var component = new T();
        ComponentList.Add(component);
        return component;
    }
    protected void RemoveComponent<T>() where T : CardComponentBase, new()
    {
        if (!HasComponent<T>(out var component))
            return;
        ComponentList.Remove(component);
    }
    
    CardUpgradeInfo CurUpgradeInfo => Config.Upgrades[UpgradeLevel];
}


[AttributeUsage(AttributeTargets.Class)]
public class CardIDAttribute(int id) : Attribute
{
    public readonly int ID = id;
}
