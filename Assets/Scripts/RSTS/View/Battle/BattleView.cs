using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class BattleView : ViewBase
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

    void BindGame(GameData gameData, StateFunc<EGameState> stateFunc)
    {
        stateFunc(EGameState.Battle)
            .OnEnterAfter(() =>
            {
                InfoView.gameObject.SetActive(true);
                PnlItem.SetActive(true);
            })
            .OnExitBefore(() =>
            {
                PnlItem.SetActive(false);
                InfoView.gameObject.SetActive(false);
            });
    }
    void BindBattle(BattleData battleData, StateFunc<EBattleState> stateFunc)
    {
        stateFunc(EBattleState.SelectLastBuff)
            .OnEnterAfter(() =>
            {
                // InitBuffButtons();
                PnlSelectLastBuff.SetActive(true);
            })
            .OnExitBefore(() =>
            {
                PnlSelectLastBuff.SetActive(false);
            });
        
        // 跳过SelectRoom阶段。
        // MyFSM.GetBindState(BattleStateWrap.One, EBattleState.SelectRoom)
        //     .OnEnter(() => MyFSM.EnterState(BattleStateWrap.One, EBattleState.BothTurn));
        
        stateFunc(EBattleState.BothTurn)
            .OnEnterAfter(() =>
            {
                PnlCard.SetActive(true);
            })
            .OnExitBefore(() =>
            {
                PnlCard.SetActive(false);
                PnlDiscardOnceClick.SetActive(false);
                PrtDiscardOnceClick.ClearActiveChildren();
            });

        stateFunc(EBattleState.Win)
            .OnEnterAfter(() =>
            {
                PnlWin.SetActive(true);
            })
            .OnExitBefore(() =>
            {
                PnlWin.SetActive(false);
            });
    }
    IEnumerable<BindDataBase> CanUnbindBattle(BattleData battleData)
    {
        foreach (var btn in LastBuffBtnList) 
            yield return Binder.FromEvt(btn.onClick).To(() => battleData.EnterState(EBattleState.BothTurn));
        yield return Binder.FromEvt(BtnReturnToTitle.onClick).To(() => FSM.GameData.EnterState(EGameState.Title));
    }

    readonly Dictionary<CardInTurn, CardModel> handCardModelDic = [];
    void BindBothTurn(BothTurnData bothTurnData, StateFunc<EBothTurnState> stateFunc)
    {
        stateFunc(EBothTurnState.PlayerYieldCard)
            .OnEnterAfter(() => 
            {
                BtnEndTurn.enabled = true;
                TxtEndTurn.text = "结束回合";
            })
            .OnExitBefore(() => 
            {
                BtnEndTurn.enabled = false;
                TxtEndTurn.text = "--";
            });
        bothTurnData.PlayerHPAndBuffData.OnAddBuff += _ =>
        {
            handCardModelDic.Values.ForEach(cardModel => cardModel.RefreshTxtDes());
        };
        bothTurnData.PlayerHPAndBuffData.OnRemoveBuff += _ =>
        {
            handCardModelDic.Values.ForEach(cardModel => cardModel.RefreshTxtDes());
        };
        bothTurnData.PlayerHPAndBuffData.OnChangeBuffStack += (_, _) =>
        {
            handCardModelDic.Values.ForEach(cardModel => cardModel.RefreshTxtDes());
        };
        bothTurnData.PlayerBlock.OnValueChangedAfter += _ =>
        {
            handCardModelDic.Values.ForEach(cardModel => cardModel.RefreshTxtDes());
        };
        
        
        bothTurnData.HandList.OnAdd += cardInTurn =>
        {
            var cardModel = Instantiate(PfbCard, PrtHandCard);
            cardModel.ReadDataInBothTurn(cardInTurn, bothTurnData);
            cardInTurn.OnTempUpgrade += cardModel.RefreshTxtDes;
            cardInTurn.OnTempUpgrade += cardModel.RefreshTxtCost;
            
            handCardModelDic.Add(cardInTurn, cardModel);
            handCardModelDic.Values.ForEach(cardModel2 => cardModel2.RefreshTxtDes());
        };
        bothTurnData.HandList.OnRemove += cardInTurn =>
        {
            // 打出破灭时，抽牌堆顶部被打出的牌没有CardModel。
            if (handCardModelDic.ContainsKey(cardInTurn))
            {
                var cardModel = handCardModelDic[cardInTurn];
                cardInTurn.OnTempUpgrade -= cardModel.RefreshTxtDes;
                cardInTurn.OnTempUpgrade -= cardModel.RefreshTxtCost;
                Destroy(cardModel.gameObject);
                handCardModelDic.Remove(cardInTurn);
            }
            handCardModelDic.Values.ForEach(cardModel2 => cardModel2.RefreshTxtDes());
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
                cardModel.ReadDataInBothTurn(cardData, bothTurnData);
                cardModel.Btn.onClick.AddListener(() =>
                {
                    onClick(cardData);
                    PnlDiscardOnceClick.SetActive(false);
                });
            }
        };

        bothTurnData.OnOpenHandOnceClick += (cardList, count, onConfirm) =>
        {
            
        };
        
        bothTurnData.OnPlayerLoseHP += () =>
        {
            handCardModelDic.Values.ForEach(cardModel => cardModel.RefreshTxtCost());
        };
    }
    IEnumerable<BindDataBase> CanUnbindBothTurn(BothTurnData bothTurnData)
    {
        yield return Binder.FromObs(bothTurnData.CurEnergy)
            .To(v => ShowEnergy(v, bothTurnData.MaxEnergy));
        yield return Binder.FromObs(bothTurnData.MaxEnergy)
            .To(v => ShowEnergy(bothTurnData.CurEnergy, v));
        yield return Binder.FromEvt(BtnEndTurn.onClick).To(() => bothTurnData.EnterState(EBothTurnState.PlayerTurnEnd));
        yield break;
        void ShowEnergy(int cur, int max) => TxtEnergy.text = cur + " / " + max;
    }
    
    IEnumerable<BindDataBase> CanUnbindYieldCard(YieldCardData yieldCardData)
    {
        var bothTurnData = yieldCardData.Parent;
        Vector3 initThisPos = default;
        Vector3 initPointerPos = default;
        Vector3 initScale = default;
        foreach(var cardModel in handCardModelDic.Values)
        {
            var cardData = cardModel.Data.InTurn;
            yield return Binder.FromEvt(cardModel.OnPointerEnterEvt).To(() =>
            {
                if (yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                if (yieldCardData.Parent.Parent.WeatherData.IsSubState<GoodData>(out var data))
                {
                    var @int = data.GetInt();
                    MyDebug.Log($"Enter When Good Weather{@int}");
                    yieldCardData.Parent.Parent.WeatherData.EnterState(EWeatherState.Bad);
                }
                else if (yieldCardData.Parent.Parent.WeatherData.IsSubState<BadData>(out var badData))
                {
                    var @int = badData.GetInt2();
                    MyDebug.Log($"Enter When Bad Weather{@int}");
                    yieldCardData.Parent.Parent.WeatherData.EnterState(EWeatherState.Good);
                }
                CurDragCard.ReadDataInBothTurn(cardData, bothTurnData);
                initThisPos = CurDragCard.transform.position = cardModel.transform.position;
                var initDeltaScale = cardModel.GetComponent<RectTransform>().sizeDelta.x
                                     / CurDragCard.GetComponent<RectTransform>().sizeDelta.x;
                initScale = cardModel.transform.localScale;
                CurDragCard.transform.localScale = initScale * (initDeltaScale * 1.5f);
            });
            yield return Binder.FromEvt(cardModel.OnPointerExitEvt).To(() =>
            {
                if (yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                CurDragCard.transform.localScale = initScale;
                CurDragCard.gameObject.SetActive(false);
            });
            yield return Binder.FromEvt(cardModel.OnBeginDragEvt).To(worldPos =>
            {
                if (yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                yieldCardData.CardModel = CurDragCard;
                yieldCardData.CardData = cardData;
                CharacterModelHolder.HidePlayerWarning();
                if (!bothTurnData.TryYield(cardData, out var failReason))
                {
                    CharacterModelHolder.ShowPlayerWarning(failReason);
                    return;
                }

                cardModel.EnableAllShown(false);
                yieldCardData.EnterState(EYieldCardState.Drag);
                initPointerPos = worldPos;
                if (!cardData.Parent.HasTarget)
                {
                    CharacterModelHolder.EnableNoTargetArea(true);
                }
            });
            yield return Binder.FromEvt(cardModel.OnDragEvt).To(worldPos =>
            {
                if (!yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                var delta = worldPos - initPointerPos;
                CurDragCard.transform.position =
                    new Vector3(initThisPos.x + delta.x, initThisPos.y + delta.y, initThisPos.z);
            });
            yield return Binder.FromEvt(cardModel.OnEndDragEvt).To(screenPos =>
            {
                if (!yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                cardModel.EnableAllShown(true);
                yieldCardData.EnterState(EYieldCardState.None);

                CurDragCard.gameObject.SetActive(false);
                if (!cardData.Parent.HasTarget)
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
                    if (cardData.Target == null)
                    {
                        CharacterModelHolder.ShowPlayerWarning("没有指向任何目标");
                        return;
                    }
                }

                bothTurnData.YieldHandAsync(cardData).Forget();
                cardData.Target = null;
            });
        }
    }

    public override void Bind()
    {
        GameData.OnRegister(alwaysBind: BindGame);
        BattleData.OnRegister(
            alwaysBind: BindBattle,
            canUnbind: CanUnbindBattle
        );
        BothTurnData.OnRegister(
            alwaysBind: BindBothTurn,
            canUnbind: CanUnbindBothTurn);
        YieldCardData.OnRegister(canUnbind: CanUnbindYieldCard);
    }
}