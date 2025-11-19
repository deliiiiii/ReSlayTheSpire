using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;



public class CharacterModelHolder : ViewBase
{
    public RectHolder TransNoTargetArea;
    public GameObject PnlPlayerWarning;
    public Text TxtPlayerWarning;
    public PlayerModel PlayerModel;

    public EnemyModel PrbEnemy;
    public List<Transform> TransEnemy = [];
    public List<EnemyModel> EnemyModelList = [];
    readonly Dictionary<EnemyDataBase, EnemyModel> enemyModelDic = [];

    readonly List<EnemyModel> enteredTargetEnemyModels = [];
    
    public bool CheckInNoTarget(Vector2 screenPos) => PosInRect(screenPos, TransNoTargetArea.RectTransform);

    public void EnableNoTargetArea(bool enable)
    {
        TransNoTargetArea.gameObject.SetActive(enable);
    }

    public void ShowPlayerWarning(string warning)
    {
        PnlPlayerWarning.SetActive(true);
        TxtPlayerWarning.text = warning;
    }

    public void HidePlayerWarning()
    {
        PnlPlayerWarning.SetActive(false);
    }
    
    void BindBattle(BattleData battleData, StateFunc<EBattleState> stateFunc)
    {
        stateFunc(EBattleState.BothTurn)
            .OnEnterAfter(() =>
            {
                PlayerModel.gameObject.SetActive(true);
            })
            .OnExitBefore(() =>
            {
                PlayerModel.gameObject.SetActive(false);
                HidePlayerWarning();
            });
    }
    
    void BindBothTurn(BothTurnData bothTurnData, StateFunc<EBothTurnState> stateFunc)
    {
        PlayerModel.HPAndBuffModel.ReadData(bothTurnData.PlayerHPAndBuffData);
        bothTurnData.EnemyList.OnAdd += enemyData =>
        {
            // TODO 应对怪物个数不同的情况
            var enemyModel = Instantiate(PrbEnemy, TransEnemy[bothTurnData.EnemyList.Count - 1]);
            enemyModel.ReadData(enemyData);
            enemyModel.gameObject.SetActive(true);
            EnemyModelList.Add(enemyModel);
            enemyModelDic.Add(enemyData, enemyModel);
        };
        
        bothTurnData.EnemyList.OnRemove += enemyData =>
        {
            if (enemyModelDic.TryGetValue(enemyData, out var enemyModel))
            {
                EnemyModelList.Remove(enemyModel);
                enemyModelDic.Remove(enemyData);
                Destroy(enemyModel.gameObject);
            }
        };
    }

    void BindYieldCard(YieldCardData yieldCardData, StateFunc<EYieldCardState> stateFunc)
    {
        stateFunc(EYieldCardState.Drag)
            .OnEnterAfter(() =>
            {
                EnemyModelList.ForEach(m =>
                {
                    m.EnableSelectTarget(false);
                });
                enteredTargetEnemyModels.Clear();
            })
            .OnExitBefore(() =>
            {
                EnemyModelList.ForEach(m =>
                {
                    m.EnableSelectTarget(false);
                });
                enteredTargetEnemyModels.Clear();
            });
    }
    
    IEnumerable<BindDataBase> CanUnbindYieldCard(YieldCardData yieldCardData)
    {
        foreach (var enemyModel in enemyModelDic.Values)
        {
            yield return Binder.FromEvt(enemyModel.OnPointerEnterEvt).To(() =>
            {
                if (!yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                if (!yieldCardData.CardData.HasTarget)
                    return;
                yieldCardData.CardData.Target = enemyModel.Data;
                yieldCardData.CardModel.RefreshTxtDes();
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(false);
                enemyModel.EnableSelectTarget(true);
                enteredTargetEnemyModels.Add(enemyModel);
            });

            yield return Binder.FromEvt(enemyModel.OnPointerExitEvt).To(() =>
            {
                if (!yieldCardData.IsState(EYieldCardState.Drag))
                    return;
                yieldCardData.CardData.Target = null;
                yieldCardData.CardModel.RefreshTxtDes();
                enteredTargetEnemyModels.Remove(enemyModel);
                enemyModel.EnableSelectTarget(false);
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(true);
            });
        }
    }

    static bool PosInRect(Vector2 pos, RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, Camera.main);
    }

    public override void Bind()
    {
        BattleData.OnRegister(alwaysBind: BindBattle);
        BothTurnData.OnRegister(alwaysBind: BindBothTurn);
        YieldCardData.OnRegister(alwaysBind: BindYieldCard, canUnbind: CanUnbindYieldCard);
    }
}