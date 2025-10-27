using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.UI;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;



public class CharacterModelHolder : Singleton<CharacterModelHolder>
{
    public RectHolder TransNoTargetArea;
    public GameObject PnlPlayerWarning;
    public Text TxtPlayerWarning;
    public PlayerModel PlayerModel;

    public EnemyModel PrbEnemy;
    public List<Transform> TransEnemy = [];
    public List<EnemyModel> EnemyModelList = [];

    readonly List<EnemyModel> enteredTargetEnemyModels = [];
    
    public EnemyDataBase? TargetingEnemy => enteredTargetEnemyModels.LastOrDefault()?.Data;
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
    
    void BindBattle(MyFSM<EBattleState> fsm, BattleData battleData)
    {
        fsm.GetState(EBattleState.BothTurn).OnEnter += () =>
        {
            PlayerModel.gameObject.SetActive(true);
        };
        fsm.GetState(EBattleState.BothTurn).OnExit += () =>
        {
            PlayerModel.gameObject.SetActive(false);
            HidePlayerWarning();
        };
    }
    
    void BindBothTurn(MyFSM<EBothTurn> fsm, BothTurnData bothTurnData)
    {
        PlayerModel.ReadData(bothTurnData.PlayerHPAndBuffData);
        bothTurnData.EnemyList.OnAdd += enemyData =>
        {
            // TODO 应对怪物个数不同的情况
            var m = Instantiate(PrbEnemy, TransEnemy[bothTurnData.EnemyList.Count - 1]);
            m.ReadData(enemyData);
            m.gameObject.SetActive(true);
            EnemyModelList.Add(m);
            
            // MyDebug.Log($"EnemyModel {m.name} find Enter Drag");
            m.OnPointerEnterEvt += () =>
            {
                // MyDebug.Log($"EnemyModel {m.name} OnPointerEnterEvt try...");
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out var yieldCardData))
                    return;
                if (!yieldCardData.CardModel.Data.HasTarget)
                    return;
                yieldCardData.CardModel.Data.Target = m.Data;
                yieldCardData.CardModel.RefreshTxtDes();
                // MyDebug.Log($"EnemyModel {m.name} OnPointerEnterEvt success!");
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(false);
                m.EnableSelectTarget(true);
                enteredTargetEnemyModels.Add(m);
            };

            m.OnPointerExitEvt += () =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out var yieldCardData))
                    return;
                yieldCardData.CardModel.Data.Target = null;
                yieldCardData.CardModel.RefreshTxtDes();
                enteredTargetEnemyModels.Remove(m);
                m.EnableSelectTarget(false);
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(true);
            };
        };
        
        bothTurnData.EnemyList.OnRemove += enemyData =>
        {
            var enemyModel = EnemyModelList.Find(em => em.Data == enemyData);
            if (enemyModel != null)
            {
                EnemyModelList.Remove(enemyModel);
                Destroy(enemyModel.gameObject);
            }
        };
    }
    
    IEnumerable<BindDataBase> CanUnbindBothTurn(BothTurnData bothTurnData)
    {
        yield return Binder.FromObs(bothTurnData.PlayerCurHP)
            .To(v => PlayerModel.HPAndBuffModel.DirectlySetHP(v, bothTurnData.PlayerMaxHP));
        yield return Binder.FromObs(bothTurnData.PlayerMaxHP)
            .To(v => PlayerModel.HPAndBuffModel.DirectlySetHP(bothTurnData.PlayerCurHP, v));
        yield return Binder.FromObs(bothTurnData.PlayerBlock)
            .To(v => PlayerModel.HPAndBuffModel.SetShield(v));
    }

    void BindYieldCard(MyFSM<EYieldCardState> fsm, YieldCardData yieldCardData)
    {
        fsm.GetState(EYieldCardState.Drag).OnExit += () =>
        {
            EnemyModelList.ForEach(m =>
            {
                m.EnableSelectTarget(false);
            });
            enteredTargetEnemyModels.Clear();
        };
    }
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(BattleStateWrap.One, alwaysBind: BindBattle);
        MyFSM.OnRegister(BothTurnStateWrap.One, 
            alwaysBind: BindBothTurn,
            canUnbind: CanUnbindBothTurn
            );
        MyFSM.OnRegister(YieldCardStateWrap.One, alwaysBind: BindYieldCard);
    }

    static bool PosInRect(Vector2 pos, RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, Camera.main);
    }
}