using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemBase
{
    public SystemBase()
    {
        Init();
    }
    protected virtual void InitRes(){}
    protected virtual void InitData(){}
    protected virtual void InitFSM(){}
    void Init()
    {
        MyDebug.Log("SystemBase Init");
        InitRes();
        InitData();
        InitFSM();
    }
}

public class SystemUpdate : SystemBase
{
    protected virtual void Update(float dt)
    {
    }
}

public class BattleSystem : SystemUpdate
{
    // MyFSM battleFSM;
    // PlayerData playerData;

    // protected override void InitRes()
    // {
    // }
    // protected override void InitData()
    // {
        
    // }

    // protected override void InitFSM()
    // {
    //     // AssetBundle ab;
    //     // GameObject prefabUICard;
    //     // ab = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/prefabs");
    //     // prefabUICard = ab.LoadAsset<GameObject>("UICard");
    //     // GameObject card = Instantiate(prefabUICard);
    // }

    protected override void Update(float dt)
    {
        // MyDebug.Log("BattleSystem Update" + updateP);
    }
}


public class ConstUICard
{
    public SerializableDictionary<Deli.CardColor, Color> cardColorToImageColor;
    public Font cardNameFont;
    public Font cardCostFont;
    public Font cardDescriptionFont;
    public string ResPath { get; set; }
}