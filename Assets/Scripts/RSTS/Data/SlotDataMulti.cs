using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;


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
public class SlotDataMulti : IMultiRef
{
    public string PlayerName;
    public int Coin;
    public EPlayerJob Job;
    public List<BottleData> BottleList = [];
    public List<CardData> CardList = [];
    public List<ItemData> ItemList = [];
    
    public MapData MapData;

    public void EnterBattle(EPlayerJob job)
    {
        Job = job;
        var config = RefPoolMulti<CardListConfigMulti>.Acquire().First(c => c.Job == job);
        config.InitialCards.ForEach(c =>
        {
            CardList.Add(new CardData(c));
        });
    }
}