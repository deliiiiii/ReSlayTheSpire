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
    
    [ShowInInspector]
    Vector3 initThisPos;
    [ShowInInspector]
    Vector3 initPointerPos;
    Vector3 initScale;
    bool isDragging;
    public CardModel CurDragCard;
    
    public Button BtnEndTurn;
    
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, s =>
        {
            GetGameStateBinds(s).BindAll();
            InfoView.GetGameStateBinds().BindAll();
        });
        MyFSM.OnRelease(GameStateWrap.One, () =>
        {
            GetGameStateBinds(null).UnBindAll();
            InfoView.GetGameStateBinds().UnBindAll();
        });
        
        MyFSM.OnRegister(BattleStateWrap.One, b => GetBattleStateBinds(b).BindAll());
        MyFSM.OnRelease(BattleStateWrap.One, () => GetBattleStateBinds(null).UnBindAll());
        
        MyFSM.OnRegister(BothTurnStateWrap.One, b => GetBothTurnStateBinds(b).BindAll());
        MyFSM.OnRelease(BothTurnStateWrap.One, () => GetBothTurnStateBinds(null).UnBindAll());
    }

    IEnumerable<BindDataBase> GetGameStateBinds(SlotDataMulti slotData)
    {
        yield return MyFSM.GetBindState(GameStateWrap.One, EGameState.Battle)
            .OnEnter(() =>
            {
                EBattleState savedBattleState = EBattleState.SelectLastBuff;
                var battleData = slotData.CreateBattleData(EPlayerJob.ZhanShi);
                InfoView.BindBattleData(battleData);
                battleData.InitDeck();
                InfoView.gameObject.SetActive(true);
                PnlItem.SetActive(true);
                MyFSM.Register(BattleStateWrap.One, savedBattleState, battleData);
            })
            .OnExit(() =>
            {
                MyFSM.Release(BattleStateWrap.One);
                PnlItem.SetActive(false);
                InfoView.gameObject.SetActive(false);
            });
    }

    IEnumerable<BindDataBase> GetBattleStateBinds(SlotDataMulti.BattleData battleData)
    {
        yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectLastBuff)
            .OnEnter(() =>
            {
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExit(() => PnlSelectLastBuff.SetActive(false));

        foreach (var btn in LastBuffBtnList)
            yield return Binder.From(btn).To(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.SelectRoom));
        
        // Select Room

        yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectRoom)
            .OnEnter(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        
        yield return MyFSM.GetBindState(BattleStateWrap.One, EBattleState.BothTurn)
            .OnEnter(() =>
            {
                var bothTurnData = battleData.CreateBothTurnData();
                BindBothTurnData(bothTurnData);
                CharacterModelHolder.BindBothTurnData(bothTurnData);
                bothTurnData.Init();
                PnlCard.SetActive(true);
                MyFSM.Register(BothTurnStateWrap.One, EBothTurn.Start_BeforeDraw, bothTurnData);
            })
            .OnExit(() =>
            {
                MyFSM.Release(BothTurnStateWrap.One);
                PnlCard.SetActive(false);
                CharacterModelHolder.OnExitBothTurn();
            });
    }
    
    IEnumerable<BindDataBase> GetBothTurnStateBinds(SlotDataMulti.BattleData.BothTurnData bothTurnData)
    {
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.Start_BeforeDraw)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDraw));
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDraw)
            .OnEnter(() =>
            {
                PrtHandCard.DestroyActiveChildren();
                bothTurnData.DrawSome(5);
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard);
            });

        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldCard)
            .OnEnter(() =>
            {
                BtnEndTurn.enabled = true;
            })
            .OnExit(() =>
            {
                BtnEndTurn.enabled = false;
            });
        yield return Binder.From(BtnEndTurn).To(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd));
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerYieldEnd)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard));
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard)
            .OnEnter(() =>
            {
                bothTurnData.DiscardAllHand();
                MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.End_AfterDiscard);
            });
        
        yield return MyFSM.GetBindState(BothTurnStateWrap.One, EBothTurn.End_AfterDiscard)
            .OnEnter(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.Start_BeforeDraw));
    }

    void BindBothTurnData(SlotDataMulti.BattleData.BothTurnData bothTurnData)
    {
        bothTurnData.HandList.OnAdd += cardData =>
        {
            var cardModel = Instantiate(PfbCard, PrtHandCard);
            cardModel.InitByData(cardData);
            cardModel.OnPointerEnterEvt += () =>
            {
                if (isDragging)
                    return;
                CurDragCard.gameObject.SetActive(true);
                CurDragCard.InitByData(cardModel.Data);
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
            cardModel.OnBeginDragEvt += pos =>
            {
                isDragging = true;
                initPointerPos = pos;
                // if(CurDragCard.Data.HasTarget)
                CharacterModelHolder.EnableNoTargetArea(true);
            };
            cardModel.OnDragEvt += curPointerPos =>
            {
                var delta = curPointerPos - initPointerPos;
                CurDragCard.transform.position =
                    new Vector3(initThisPos.x + delta.x, initThisPos.y + delta.y, initThisPos.z);
            };
            cardModel.OnEndDragEvt += pos =>
            {
                isDragging = false;
                CurDragCard.gameObject.SetActive(false);
                CharacterModelHolder.EnableNoTargetArea(false);
                if (!CharacterModelHolder.CheckInNoTarget(pos))
                {
                    MyDebug.LogError("没有拖到无目标区域，");
                    return;
                }

                if (!bothTurnData.TryYield(cardModel.Data, out var failReason))
                {
                    MyDebug.LogError($"不可打出，原因：{failReason}");
                }
            };
            cardModel.gameObject.SetActive(true);
        };

        bothTurnData.HandList.OnRemove += cardData =>
        {
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

