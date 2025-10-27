using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
    public CardModel PfbCard;
    public CardModel PfbCardOnceClick;
    
    public GameObject PnlCard;
    public Transform PrtHandCard;
    public Transform PrtDrawCard;
    public Text TxtToDrawCount;
    public Transform PrtDiscardCard;
    public GameObject PnlDiscardOnceClick;
    public Transform PrtDiscardOnceClick;
    public Text TxtHasDiscardCount;
    public Transform PrtExhaustCard;
    public Text TxtHasExhaustCount;
    
    public Text TxtEnergy;
    
    public CardModel CurDragCard;
    
    public Button BtnEndTurn;
    public Text TxtEndTurn;
    
    #endregion
    #region Win
    [Header("Win")]
    public GameObject PnlWin;
    public Button BtnReturnToTitle;
    #endregion

    void BindGame(MyFSM<EGameState> fsm, GameData gameData)
    {
        fsm.GetState(EGameState.Battle).OnEnter += () =>
        {
            InfoView.gameObject.SetActive(true);
            PnlItem.SetActive(true);
            MyFSM.Register(BattleStateWrap.One, EBattleState.SelectLastBuff,
                _ => gameData.CreateBattleData(EPlayerJob.ZhanShi));
        };
        fsm.GetState(EGameState.Battle).OnExit += () =>
        {
            MyFSM.Release(BattleStateWrap.One);
            PnlItem.SetActive(false);
            InfoView.gameObject.SetActive(false);
        };
    }
    void BindBattle(MyFSM<EBattleState> fsm, BattleData battleData)
    {
        fsm.GetState(EBattleState.SelectLastBuff).OnEnter += () =>
        {
            // InitBuffButtons();
            PnlSelectLastBuff.SetActive(true);
        };
        fsm.GetState(EBattleState.SelectLastBuff).OnExit += () =>
        {
            PnlSelectLastBuff.SetActive(false);
        };
        
        // 跳过SelectRoom阶段。
        // MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectRoom)
        //     .OnEnter(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        
        fsm.GetState(EBattleState.BothTurn).OnEnter += () =>
        {
            PnlCard.SetActive(true);
            // 跳过GrossStart、TurnStart阶段。
            MyFSM.Register(BothTurnStateWrap.One, EBothTurn.PlayerDraw, battleData.CreateBothTurnData);
        };
            
        fsm.GetState(EBattleState.BothTurn).OnExit += () =>
        {
            PnlCard.SetActive(false);
            PnlDiscardOnceClick.SetActive(false);
            PrtDiscardOnceClick.ClearActiveChildren();
            
            MyFSM.Release(BothTurnStateWrap.One);
        };

        fsm.GetState(EBattleState.Win).OnEnter += () =>
        {
            PnlWin.SetActive(true);
        };
        fsm.GetState(EBattleState.Win).OnExit += () =>
        {
            PnlWin.SetActive(false);
        };
    }
    IEnumerable<BindDataBase> CanUnbindBattle(BattleData battleData)
    {
        foreach (var btn in LastBuffBtnList) 
            yield return Binder.FromBtn(btn).To(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        yield return Binder.FromBtn(BtnReturnToTitle).To(() => MyFSM.EnterState(GameStateWrap.One, EGameState.Title));
    }
    void BindBothTurn(MyFSM<EBothTurn> fsm, BothTurnData bothTurnData)
    {
        fsm.GetState(EBothTurn.PlayerYieldCard).OnEnter += () =>
        {
            BtnEndTurn.enabled = true;
            TxtEndTurn.text = "结束回合";
            
        };
        fsm.GetState(EBothTurn.PlayerYieldCard).OnExit += () =>
        {
            BtnEndTurn.enabled = false;
            TxtEndTurn.text = "--";
        };
        
        Dictionary<CardDataBase, CardModel> handCardModelDic = new();
        bothTurnData.HandList.OnAdd += cardData =>
        {
            Vector3 initThisPos = default;
            Vector3 initPointerPos = default;
            Vector3 initScale = default;
            
            var cardModel = Instantiate(PfbCard, PrtHandCard);
            cardModel.InitByData(cardData);
            cardModel.gameObject.SetActive(true);
            handCardModelDic.Add(cardData, cardModel);
            cardModel.OnPointerEnterEvt += () =>
            {
                if (MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag))
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
                if (MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag))
                    return;
                CurDragCard.transform.localScale = initScale;
                CurDragCard.gameObject.SetActive(false);
            };
            cardModel.OnBeginDragEvt += worldPos =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.None, out var yieldCardData))
                    return;
                CharacterModelHolder.HidePlayerWarning();
                if (!bothTurnData.TryYield(cardData, out var failReason))
                {
                    CharacterModelHolder.ShowPlayerWarning(failReason);
                    return;
                }
                cardModel.EnableAllShown(false);
                MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.Drag);
                initPointerPos = worldPos;
                yieldCardData.HasTarget = cardData.HasTarget;
                if (!yieldCardData.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(true);
                }
            };
            cardModel.OnDragEvt += worldPos =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag))
                    return;
                var delta = worldPos - initPointerPos;
                CurDragCard.transform.position =
                    new Vector3(initThisPos.x + delta.x, initThisPos.y + delta.y, initThisPos.z);
            };
            cardModel.OnEndDragEvt += screenPos =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag))
                    return;
                cardModel.EnableAllShown(true);
                EnemyDataBase? targetingEnemy = CharacterModelHolder.TargetingEnemy;
                var yieldCardData = MyFSM.EnterState(YieldCardStateWrap.One, EYieldCardState.None);

                CurDragCard.gameObject.SetActive(false);
                if (!yieldCardData.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(false);
                    if (!CharacterModelHolder.CheckInNoTarget(screenPos))
                    {
                        CharacterModelHolder.ShowPlayerWarning("没有拖到无目标区域");
                        return;
                    }
                }
                else
                {
                    if (targetingEnemy == null)
                    {
                        CharacterModelHolder.ShowPlayerWarning("没有指向任何目标");
                        return;
                    }
                    cardData.Target = targetingEnemy;
                }
                bothTurnData.YieldAsync(cardData).Forget();
            };
        };
        bothTurnData.HandList.OnRemove += cardData =>
        {
            Destroy(handCardModelDic[cardData].gameObject);
            handCardModelDic.Remove(cardData);
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

        bothTurnData.OnOpenDiscardOnceClick += (cardList, onClick) =>
        {
            PnlDiscardOnceClick.SetActive(true);
            PrtDiscardOnceClick.ClearActiveChildren();
            foreach (var cardData in cardList)
            {
                var cardModel = Instantiate(PfbCardOnceClick, PrtDiscardOnceClick);
                cardModel.InitByData(cardData);
                cardModel.gameObject.SetActive(true);
                cardModel.Btn.onClick.AddListener(() =>
                {
                    onClick(cardData);
                    PnlDiscardOnceClick.SetActive(false);
                });
            }
        };
    }
    IEnumerable<BindDataBase> CanUnbindBothTurn(BothTurnData bothTurnData)
    {
        yield return Binder.FromObs(bothTurnData.CurEnergy)
            .To(v => ShowEnergy(v, bothTurnData.MaxEnergy));
        yield return Binder.FromObs(bothTurnData.MaxEnergy)
            .To(v => ShowEnergy(bothTurnData.CurEnergy, v));
        yield return Binder.FromBtn(BtnEndTurn).To(() => MyFSM.EnterState(BothTurnStateWrap.One, EBothTurn.PlayerDiscard));
        yield break;
        void ShowEnergy(int cur, int max) => TxtEnergy.text = cur + " / " + max;
    }
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, 
            alwaysBind: BindGame);
        MyFSM.OnRegister(BattleStateWrap.One, 
            alwaysBind: BindBattle,
            canUnbind: CanUnbindBattle
            );
        MyFSM.OnRegister(BothTurnStateWrap.One, 
            alwaysBind: BindBothTurn,
            canUnbind: CanUnbindBothTurn);
    }
    
}