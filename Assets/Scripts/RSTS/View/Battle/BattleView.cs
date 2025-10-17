using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, slotData =>
        {
            slotData.OnBattleDataCreate += battleData =>
            {
                BindBattleData(battleData);
                InfoView.BindBattleData(battleData);
            };
            GetGameStateBinds(slotData).BindAll();
            InfoView.GetGameStateBinds().BindAll();
        });
        MyFSM.OnRelease(GameStateWrap.One, @null =>
        {
            GetGameStateBinds(@null).UnBindAll();
            InfoView.GetGameStateBinds().UnBindAll();
        });
        
        MyFSM.OnRegister(BattleStateWrap.One, battleData => GetBattleStateBinds(battleData).BindAll());
        MyFSM.OnRelease(BattleStateWrap.One, @null => GetBattleStateBinds(@null).UnBindAll());
        
        MyFSM.OnRegister(BothTurnStateWrap.One, bothTurnData => GetBothTurnStateBinds(bothTurnData).BindAll());
        MyFSM.OnRelease(BothTurnStateWrap.One, @null => GetBothTurnStateBinds(@null).UnBindAll());
        
        MyFSM.OnRegister(YieldCardStateWrap.One, bothTurnData => { CharacterModelHolder.GetPlayerYieldStateBinds(bothTurnData).BindAll();});
        MyFSM.OnRelease(YieldCardStateWrap.One, bothTurnData => { CharacterModelHolder.GetPlayerYieldStateBinds(bothTurnData).UnBindAll();});
    }

    IEnumerable<BindDataBase> GetGameStateBinds(SlotDataMulti slotData)
    {
        yield return MyFSM.GetBindState(GameStateWrap.One, EGameState.Battle)
            .OnEnter(() =>
            {
                InfoView.gameObject.SetActive(true);
                PnlItem.SetActive(true);
                MyFSM.Register(BattleStateWrap.One, EBattleState.SelectLastBuff, 
                    slotData.CreateAndInitBattleData(EPlayerJob.ZhanShi));
            })
            .OnExit(() =>
            {
                MyFSM.Release(BattleStateWrap.One);
                PnlItem.SetActive(false);
                InfoView.gameObject.SetActive(false);
            });
    }

    void BindBattleData(BattleData battleData)
    {
        battleData.OnBothTurnDataCreate += bothTurnData =>
        {
            BindBothTurnData(bothTurnData);
            CharacterModelHolder.BindBothTurnData(bothTurnData);
        };
    }

    IEnumerable<BindDataBase> GetBattleStateBinds(BattleData battleData)
    {
        yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectLastBuff)
            .OnEnter(() =>
            {
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExit(() => PnlSelectLastBuff.SetActive(false));

        foreach (var btn in LastBuffBtnList)
            yield return Binder.From(btn).To(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        
        // 跳过SelectRoom阶段。
        // yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectRoom)
        //     .OnEnter(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));

        yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.BothTurn)
            .OnEnter(() =>
            {
                var bothTurnData = battleData.CreateAndInitBothTurnData();
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
    
    IEnumerable<BindDataBase> GetBothTurnStateBinds(BothTurnData bothTurnData)
    {
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.GrossStart)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.TurnStart));
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.TurnStart)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDraw));
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDraw)
            .OnEnter(() =>
            {
                bothTurnData.DrawSome(5);
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard);
            });

        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard)
            .OnEnter(() =>
            {
                BtnEndTurn.enabled = true;
                TxtEndTurn.text = "结束回合";
                
                MyFSM.Register(YieldCardStateWrap.One, EYieldCardState.None, bothTurnData);
            })
            .OnExit(() =>
            {
                BtnEndTurn.enabled = false;
                TxtEndTurn.text = "--";
                
                MyFSM.Release(YieldCardStateWrap.One);
            });
        yield return Binder.From(BtnEndTurn).To(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd));
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard));
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard)
            .OnEnter(() =>
            {
                bothTurnData.DiscardAllHand();
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.DiscardEnd);
            });
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.DiscardEnd)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.TurnStart));
    }

    void BindBothTurnData(BothTurnData bothTurnData)
    {
        // [ShowInInspector]
        Vector3 initThisPos = default;
        // [ShowInInspector]
        Vector3 initPointerPos = default;
        Vector3 initScale = default;
        bool isDragging = false;
        
        bothTurnData.HandList.OnAdd += cardData =>
        {
            var cardModel = Instantiate(PfbCard, PrtHandCard);
            cardModel.InitByData(cardData);
            cardModel.OnPointerEnterEvt += () =>
            {
                if (isDragging)
                    return;
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
                if (isDragging)
                    return;
                CurDragCard.transform.localScale = initScale;
                CurDragCard.gameObject.SetActive(false);
            };
            cardModel.OnBeginDragEvt += worldPos =>
            {
                isDragging = true;
                MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.Drag);
                initPointerPos = worldPos;
                bothTurnData.HasSelectTarget = cardData.HasTarget;
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
                MyDebug.Log("结束拖拽");
                isDragging = false;
                
                CurDragCard.gameObject.SetActive(false);
                if (!cardData.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(false);
                    if (!CharacterModelHolder.CheckInNoTarget(screenPos))
                    {
                        MyDebug.LogError("没有拖到无目标区域，");
                        MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.None);
                        return;
                    }
                }
                else
                {
                    if (!CharacterModelHolder.TargetingEnemy)
                    {
                        MyDebug.LogError("没有指向任何目标。");
                        MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.None);
                        return;
                    }
                }

                if (!bothTurnData.TryYield(cardData, out var failReason))
                {
                    MyDebug.LogError($"不可打出，原因：{failReason}");
                }
                MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.None);
            };
            cardModel.gameObject.SetActive(true);
        };

        bothTurnData.HandList.OnRemove += cardData =>
        {
            // TODO 准备改为Dic存储
            PrtHandCard.transform.DestroyChild(t => t.GetComponent<CardModel>().Data == cardData);
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