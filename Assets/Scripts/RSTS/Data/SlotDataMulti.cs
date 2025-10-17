global using BattleData = RSTS.SlotDataMulti.BattleData;
global using BothTurnData = RSTS.SlotDataMulti.BattleData.BothTurnData;
using System;
using System.Collections.Generic;
using System.Linq;
using RSTS.CDMV;
using Sirenix.Utilities;
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
public class SlotDataMulti: IMyFSMArg
{
    public string PlayerName;
    public bool HasLastBuff;
    [SerializeReference]
    BattleData battleData;
    public event Action<BattleData>? OnBattleDataCreate;
    
    public BattleData CreateAndInitBattleData(EPlayerJob job)
    {
        battleData = new BattleData(this, job);
        OnBattleDataCreate?.Invoke(battleData);
        battleData.InitDeck();
        return battleData;
    }
    
    
    [Serializable]
    public class BattleData : IMyFSMArg
    {
        [NonSerialized]
        SlotDataMulti slotData;
        
        [SerializeReference]
        public MyList<CardDataBase> DeckList = [];
        
        public List<ItemData> ItemList = [];
        public int CurHP;
        public int MaxHP;
        public int Coin;
        public EPlayerJob Job;
        public List<BottleData> BottleList = [];
        public float InBattleTime;
        [SerializeReference]MapData mapData;
        [SerializeReference]BothTurnData bothTurnData;


        public BattleData(SlotDataMulti slotData, EPlayerJob job)
        {
            this.slotData = slotData;
            Job = job;
        }
        public void InitDeck()
        {
            var config = RefPoolMulti<CardListConfigMulti>.Acquire().First(c => c.Job == Job);
            config.InitialCardDic.ForEach(pair =>
            {
                for(int i = 0; i < pair.Value; i++)
                    DeckList.MyAdd(CardDataBase.CreateCard(pair.Key.ID));
            });
        }
        
        [Serializable]
        public class MapData;
        
        [Serializable]
        public class BothTurnData : IMyFSMArg
        {
            [NonSerialized]
            BattleData battleData;
            
            public int PlayerDefend;
            public int TurnID;
            [SerializeReference]
            public MyList<EnemyDataBase> EnemyList = [];
            public bool HasSelectTarget;
            
            public BothTurnData(BattleData battleData)
            {
                MyDebug.Log("BothTurnData ctor()");
                this.battleData = battleData;
                Init();
            }
            

            public void Init()
            {
                PlayerDefend = TurnID = 0;
                EnemyList.MyClear();
                EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
                EnemyList.MyAdd(EnemyDataBase.CreateEnemy(1));
                EnemyList.MyAdd(EnemyDataBase.CreateEnemy(0));
                
                HandList.MyClear();
                DrawList.MyClear();
                DrawList.MyAddRange(battleData.DeckList);
                DrawList.Shuffle();
                DiscardList.MyClear();
                ExhaustList.MyClear();
            }

            public void UnInit()
            {
                EnemyList.MyClear();
                HandList.MyClear();
                DrawList.MyClear();
                DiscardList.MyClear();
                ExhaustList.MyClear();
            }
            
            [SerializeReference]
            public MyList<CardDataBase> HandList = [];
            [SerializeReference]
            public MyList<CardDataBase> DrawList = [];
            [SerializeReference]
            public MyList<CardDataBase> DiscardList = [];
            [SerializeReference]
            public MyList<CardDataBase> ExhaustList = [];
            public int Energy;

            public bool DrawSome(int drawCount)
            {
                for (int i = 0; i < drawCount; i++)
                {
                    if (!DrawOne())
                        return false;
                }
                return true;
            }

            bool DrawOne()
            {
                if (DrawList.Count == 0)
                    RefillDrawList();
                if (DrawList.Count == 0)
                {
                    Debug.LogError("没有牌可以抽了");
                    return false;
                }
                var ret = DrawList[^1];
                DrawList.MyRemoveAt(DrawList.Count - 1);
                HandList.MyAdd(ret);
                return true;
            }

            public bool TryYield(CardDataBase toYield, out string failReason)
            {
                failReason = string.Empty;
                if (Energy < toYield.CurUpgradeInfo.CostInfo switch
                    {
                        CardCostNumber costNumber => costNumber.Cost,
                        CardCostX => 0,
                        CardCostNone or _ => int.MaxValue,
                    })
                {
                    failReason = "能量不足";
                    return false;
                }

                if(toYield.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Unplayable))
                {
                    failReason = "该牌无法打出";
                    return false;
                }
                Yield(toYield);
                return true;
            }

            public void DiscardAllHand()
            {
                HandList.ForEach(c =>
                {
                    if(c.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Ethereal))
                        ExhaustList.MyAdd(c);
                    else if(c.Config.Category != ECardCategory.State)
                        DiscardList.MyAdd(c);
                });
                HandList.MyClear();
            }

            void Yield(CardDataBase toYield)
            {
                toYield.Yield(this);
                HandList.MyRemove(toYield);
                if (toYield.CurUpgradeInfo.Keywords.Contains(ECardKeyword.Exhaust))
                {
                    ExhaustList.MyAdd(toYield);
                }
                else
                {
                    DiscardList.MyAdd(toYield);
                }
            }
            
            
            void RefillDrawList()
            {
                DrawList.MyAddRange(DiscardList);
                DiscardList.MyClear();
                DrawList.Shuffle();
            }
        }

        public BothTurnData CreateAndInitBothTurnData()
        {
            bothTurnData = new BothTurnData(this);
            OnBothTurnDataCreate?.Invoke(bothTurnData);
            bothTurnData.Init();
            return bothTurnData;
        }

        public event Action<BothTurnData>? OnBothTurnDataCreate;
        
        public BothTurnData UnloadBothTurnData()
        {
            bothTurnData.UnInit();
            return bothTurnData;
        }
    }
}

