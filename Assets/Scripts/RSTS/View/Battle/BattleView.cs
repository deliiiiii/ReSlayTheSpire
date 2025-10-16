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
    [Header(nameof(slotData))]
    [SerializeReference][ReadOnly] SlotDataMulti slotData;
    [SerializeReference] [ReadOnly] BothTurnData bothTurnData;
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister<EGameState>(() => GetGameStateBinds().BindAll());
        MyFSM.OnRelease<EGameState>(() => GetGameStateBinds().UnBindAll());
        
        MyFSM.OnRegister<EBattleState>(() => GetBattleStateBinds().BindAll());
        MyFSM.OnRelease<EBattleState>(() => GetBattleStateBinds().UnBindAll());
        
        MyFSM.OnRegister<EBothTurn>(() => GetBothTurnStateBinds().BindAll());
        MyFSM.OnRelease<EBothTurn>(() => GetBothTurnStateBinds().UnBindAll());
    }

    IEnumerable<BindDataBase> GetGameStateBinds()
    {
        yield return MyFSM.GetBindState(EGameState.Battle)
            .OnEnter(() =>
            {
                EBattleState savedBattleState = EBattleState.BothTurn;
                slotData = new SlotDataMulti();
                if (!slotData.EverInBattle)
                {
                    savedBattleState = EBattleState.SelectLastBuff;
                }
                InfoView.OnEnterBattle(slotData);
                InfoView.gameObject.SetActive(true);
                PnlItem.SetActive(true);
                MyFSM.Register(savedBattleState);
            })
            .OnExit(() =>
            {
                MyFSM.Release<EBattleState>();
                PnlItem.SetActive(false);
                InfoView.gameObject.SetActive(false);
                slotData.ExitBattle();
            });
    }

    IEnumerable<BindDataBase> GetBattleStateBinds()
    {
        yield return MyFSM.GetBindState(EBattleState.SelectLastBuff)
            .OnEnter(() =>
            {
                slotData.FirstEnterBattle(EPlayerJob.ZhanShi);
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExit(() => PnlSelectLastBuff.SetActive(false));

        foreach (var btn in LastBuffBtnList)
            yield return Binder.From(btn).To(() => MyFSM.EnterState(EBattleState.SelectRoom));
        
        // Select Room

        yield return MyFSM.GetBindState(EBattleState.SelectRoom)
            .OnEnter(() => MyFSM.EnterState(EBattleState.BothTurn));
        
        yield return MyFSM.GetBindState(EBattleState.BothTurn)
            .OnEnter(() =>
            {
                bothTurnData = new BothTurnData();
                bothTurnData.EnterBothTurn();
                PnlCard.SetActive(true);
                slotData.CardDataHolder.OnEnterBothTurn();
                CharacterModelHolder.OnEnterBothTurn(bothTurnData);
                MyFSM.Register(EBothTurn.Start_BeforeDraw);
            })
            .OnExit(() =>
            {
                MyFSM.Release<EBothTurn>();
                CharacterModelHolder.OnExitBothTurn();
                PnlCard.SetActive(false);
            });
        
        yield return Binder.From(slotData.CardDataHolder.DrawList.OnAdd).To(cardData =>
        {
            TxtToDrawCount.text = slotData.CardDataHolder.DrawList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.DrawList.OnRemove).To(cardData =>
        {
            TxtToDrawCount.text = slotData.CardDataHolder.DrawList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.DrawList.OnClear).To(() =>
        {
            TxtToDrawCount.text = "0";
        });
        
        
        yield return Binder.From(slotData.CardDataHolder.DiscardList.OnAdd).To(cardData =>
        {
            TxtHasDiscardCount.text = slotData.CardDataHolder.DiscardList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.DiscardList.OnRemove).To(cardData =>
        {
            TxtHasDiscardCount.text = slotData.CardDataHolder.DiscardList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.DiscardList.OnClear).To(() =>
        {
            TxtHasDiscardCount.text = "0";
        });
        
        
        yield return Binder.From(slotData.CardDataHolder.ExhaustList.OnAdd).To(cardData =>
        {
            TxtHasExhaustCount.text = slotData.CardDataHolder.ExhaustList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.ExhaustList.OnRemove).To(cardData =>
        {
            TxtHasExhaustCount.text = slotData.CardDataHolder.ExhaustList.Count.ToString();
        });
        yield return Binder.From(slotData.CardDataHolder.ExhaustList.OnClear).To(() =>
        {
            TxtHasExhaustCount.text = "0";
        });
    }
    
    IEnumerable<BindDataBase> GetBothTurnStateBinds()
    {
        yield return MyFSM.GetBindState(EBothTurn.Start_BeforeDraw)
            .OnEnter(() => MyFSM.EnterState(EBothTurn.PlayerDraw));
        
        yield return MyFSM.GetBindState(EBothTurn.PlayerDraw)
            .OnEnter(() =>
            {
                PrtHandCard.DestroyActiveChildren();
                slotData.CardDataHolder.DrawSome(5);
                
                MyFSM.EnterState(EBothTurn.PlayerYieldCard);
            });
        
        yield return Binder.From(slotData.CardDataHolder.HandList.OnAdd).To(cardData =>
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

                if (!slotData.CardDataHolder.TryYield(cardModel.Data, out var failReason))
                {
                    MyDebug.LogError($"不可打出，原因：{failReason}");
                }
            };
            cardModel.gameObject.SetActive(true);
        });

        yield return Binder.From(slotData.CardDataHolder.HandList.OnRemove).To(cardData =>
        {
            PrtHandCard.transform.DestroyChild(t => t.GetComponent<CardModel>().Data == cardData);
        });

        yield return MyFSM.GetBindState(EBothTurn.PlayerYieldCard)
            .OnEnter(() =>
            {
                BtnEndTurn.enabled = true;
            })
            .OnExit(() =>
            {
                BtnEndTurn.enabled = false;
            });
        yield return Binder.From(BtnEndTurn).To(() => MyFSM.EnterState(EBothTurn.PlayerYieldEnd));
        yield return MyFSM.GetBindState(EBothTurn.PlayerYieldEnd)
            .OnEnter(() => MyFSM.EnterState(EBothTurn.PlayerDiscard));
        
        yield return MyFSM.GetBindState(EBothTurn.PlayerDiscard)
            .OnEnter(() =>
            {
                slotData.CardDataHolder.DiscardAllHand();
                MyFSM.EnterState(EBothTurn.End_AfterDiscard);
            });
        
        yield return MyFSM.GetBindState(EBothTurn.End_AfterDiscard)
            .OnEnter(() => MyFSM.EnterState(EBothTurn.Start_BeforeDraw));
    }
}

