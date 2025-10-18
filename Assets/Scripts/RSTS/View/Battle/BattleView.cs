using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class BattleView : Singleton<BattleView>
{
    public InfoView InfoView;
    
    [Header("Item")]
    public GameObject PnlItem;
    
    [Header("Model")]
    public CharacterModelHolder CharacterModelHolder;
    
    #region Select Last Buff
    [Header("Select Last Buff")]
    public GameObject PnlSelectLastBuff;
    public List<Button> LastBuffBtnList;
    #endregion
    
    // #region Select Room
    // [Header("Select Room")]
    // public GameObject PnlSelectRoom;
    // #endregion
    
    #region Yield Card
    [Header("Yield Card")]
    public GameObject PnlCard;
    public Transform PrtHandCard;
    public CardModel PfbCard;
    public Text TxtToDrawCount;
    public Text TxtHasDiscardCount;
    public Text TxtHasExhaustCount;
    
    public CardModel CurDragCard;
    
    public Button BtnEndTurn;
    public Text TxtEndTurn;
    
    #endregion

    IEnumerable<BindDataBase> OnRegisterGameState(SlotData slotData)
    {
        // MyDebug.Log("OnRegisterGameState");
        slotData.OnBattleDataCreate += battleData =>
        {
            // MyDebug.Log("OnBattleDataCreate");
            battleData.OnBothTurnDataCreate += bothTurnData =>
            {
                // MyDebug.Log("OnBothTurnDataCreate");
                OnCreateBothTurnData(bothTurnData);
                CharacterModelHolder.OnCreateBothTurnData(bothTurnData);
            };
            InfoView.OnCreateBattleData(battleData);
        };
        yield break;
    }
    void OnChangeGameState(SlotData slotData)
    {
        MyFSM.GetBindState(GameStateWrap.One, EGameState.Battle)
            .OnEnter(() =>
            {
                InfoView.gameObject.SetActive(true);
                PnlItem.SetActive(true);
                MyFSM.Register(BattleStateWrap.One, EBattleState.SelectLastBuff,
                    slotData.CreateBattleData(EPlayerJob.ZhanShi));
            })
            .OnExit(() =>
            {
                MyFSM.Release(BattleStateWrap.One);
                PnlItem.SetActive(false);
                InfoView.gameObject.SetActive(false);
            });
    }
    
    IEnumerable<BindDataBase> OnRegisterBattleState(BattleData battleData)
    {
        foreach (var btn in LastBuffBtnList) 
            yield return Binder.From(btn).To(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        foreach (var b in InfoView.OnRegisterBattleState(battleData))
            yield return b;
    }
    void OnChangeBattleState(BattleData battleData)
    {
        MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectLastBuff)
            .OnEnter(() =>
            {
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExit(() => PnlSelectLastBuff.SetActive(false));
        
        // 跳过SelectRoom阶段。
        // MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectRoom)
        //     .OnEnter(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        
        MyFSM.GetBindState(BattleStateWrap.One, EBattleState.BothTurn)
            .OnEnter(() =>
            {
                var bothTurnData = battleData.CreateBothTurnData();
                PnlCard.SetActive(true);
                // 跳过GrossStart、TurnStart阶段。
                MyFSM.Register(BothTurnStateWrap.One, EBothTurn.PlayerDraw, bothTurnData);
            })
            .OnExit(() =>
            {
                battleData.UnloadBothTurnData();
                PnlCard.SetActive(false);
                MyFSM.Release(BothTurnStateWrap.One);
            });
    }

    IEnumerable<BindDataBase> OnRegisterBothTurnState(BothTurnData bothTurnData)
    {
        yield return Binder.From(BtnEndTurn).To(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd));
    }
    void OnChangeBothTurnState(BothTurnData bothTurnData)
    {
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.GrossStart)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.TurnStart));
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.TurnStart)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDraw));
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDraw)
            .OnEnter(() =>
            {
                bothTurnData.DrawSome(5);
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard);
            });
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard)
            .OnEnter(() =>
            {
                BtnEndTurn.enabled = true;
                TxtEndTurn.text = "结束回合";
                
                MyFSM.Register(YieldCardStateWrap.One, EYieldCardState.None, bothTurnData.CreateYieldCardData());
            })
            .OnExit(() =>
            {
                BtnEndTurn.enabled = false;
                TxtEndTurn.text = "--";
                
                MyFSM.Release(YieldCardStateWrap.One);
            });
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard));
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard)
            .OnEnter(() =>
            {
                bothTurnData.DiscardAllHand();
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.DiscardEnd);
            });
        MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.DiscardEnd)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.TurnStart));
    }
    
    IEnumerable<BindDataBase> OnRegisterYieldCardState(YieldCardData arg)
    {
        yield break;
    }
    void OnChangeYieldCardState(YieldCardData yieldCardData)
    {
        CharacterModelHolder.OnChangeYieldCardState(yieldCardData);
    }
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, OnRegisterGameState, OnChangeGameState);
        MyFSM.OnRegister(BattleStateWrap.One, OnRegisterBattleState, OnChangeBattleState);
        MyFSM.OnRegister(BothTurnStateWrap.One, OnRegisterBothTurnState, OnChangeBothTurnState);
        MyFSM.OnRegister(YieldCardStateWrap.One, OnRegisterYieldCardState, OnChangeYieldCardState);
    }

    void OnCreateBothTurnData(BothTurnData bothTurnData)
    {
        bothTurnData.HandList.OnAdd += cardData =>
        {
            // MyDebug.Log("BothTurnData HandList OnAdd");
            Vector3 initThisPos = default;
            Vector3 initPointerPos = default;
            Vector3 initScale = default;
            
            var cardModel = Instantiate(PfbCard, PrtHandCard);
            cardModel.InitByData(cardData);
            cardModel.gameObject.SetActive(true);
            cardModel.OnPointerEnterEvt += () =>
            {
                if (MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out _))
                    return;
                // MyDebug.Log($"第 {cardModel.transform.GetSiblingIndex()} 张牌 OnPointerEnterEvt");
                CurDragCard.gameObject.SetActive(true);
                CurDragCard.InitByData(cardData);
                initThisPos = CurDragCard.transform.position = cardModel.transform.position;
                var initDeltaScale = cardModel.GetComponent<RectTransform>().sizeDelta.x
                                     / CurDragCard.GetComponent<RectTransform>().sizeDelta.x;
                initScale = cardModel.transform.localScale;
                CurDragCard.transform.localScale = initScale * (initDeltaScale * 1.5f);
            };
            cardModel.OnPointerExitEvt += () =>
            {
                if (MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out _))
                    return;
                CurDragCard.transform.localScale = initScale;
                CurDragCard.gameObject.SetActive(false);
            };
            cardModel.OnBeginDragEvt += worldPos =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.None, out var yieldCardData))
                    return;
                MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.Drag);
                initPointerPos = worldPos;
                yieldCardData.HasTarget = cardData.HasTarget;
                if (!cardData.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(true);
                }
            };
            cardModel.OnDragEvt += worldPos =>
            {
                var delta = worldPos - initPointerPos;
                CurDragCard.transform.position =
                    new Vector3(initThisPos.x + delta.x, initThisPos.y + delta.y, initThisPos.z);
            };
            cardModel.OnEndDragEvt += screenPos =>
            {
                // MyDebug.Log("结束拖拽");
                bool targetingEnemy = CharacterModelHolder.TargetingEnemy;
                MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.None);

                CurDragCard.gameObject.SetActive(false);
                if (!cardData.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(false);
                    if (!CharacterModelHolder.CheckInNoTarget(screenPos))
                    {
                        MyDebug.LogError("没有拖到无目标区域，");
                        return;
                    }
                }
                else
                {
                    if (!targetingEnemy)
                    {
                        MyDebug.LogError("没有指向任何目标。");
                        return;
                    }
                }

                if (!bothTurnData.TryYield(cardData, out var failReason))
                {
                    MyDebug.LogError($"不可打出，原因：{failReason}");
                }
            };
        };

        bothTurnData.HandList.OnRemove += cardData =>
        {
            // TODO 准备改为Dic存储
            PrtHandCard.DestroyChild(t => t.GetComponent<CardModel>().Data == cardData);
        };
        
        bothTurnData.DrawList.OnAdd += cardData =>
        {
            TxtToDrawCount.text = bothTurnData.DrawList.Count.ToString();
        };
        bothTurnData.DrawList.OnRemove += cardData =>
        {
            TxtToDrawCount.text = bothTurnData.DrawList.Count.ToString();
        };
        bothTurnData.DrawList.OnClear += () =>
        {
            TxtToDrawCount.text = "0";
        };
        bothTurnData.DiscardList.OnAdd += cardData =>
        {
            TxtHasDiscardCount.text = bothTurnData.DiscardList.Count.ToString();
        };
        bothTurnData.DiscardList.OnRemove += cardData =>
        {
            TxtHasDiscardCount.text = bothTurnData.DiscardList.Count.ToString();
        };
        bothTurnData.DiscardList.OnClear += () =>
        {
            TxtHasDiscardCount.text = "0";
        };
        bothTurnData.ExhaustList.OnAdd += cardData =>
        {
            TxtHasExhaustCount.text = bothTurnData.ExhaustList.Count.ToString();
        };
        bothTurnData.ExhaustList.OnRemove += cardData =>
        {
            TxtHasExhaustCount.text = bothTurnData.ExhaustList.Count.ToString();
        };
        bothTurnData.ExhaustList.OnClear += () =>
        {
            TxtHasExhaustCount.text = "0";
        };
    }
}