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
    readonly Dictionary<EnemyDataBase, EnemyModel> enemyModelDic = [];

    readonly List<EnemyModel> enteredTargetEnemyModels = [];

    public CharacterModelHolder()
    {
    }

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
        PlayerModel.HPAndBuffModel.ReadData(bothTurnData.PlayerHPAndBuffData);
        bothTurnData.EnemyList.OnAdd += enemyData =>
        {
            // TODO 应对怪物个数不同的情况
            var enemyModel = Instantiate(PrbEnemy, TransEnemy[bothTurnData.EnemyList.Count - 1]);
            enemyModel.ReadData(enemyData);
            enemyModel.gameObject.SetActive(true);
            EnemyModelList.Add(enemyModel);
            enemyModelDic.Add(enemyData, enemyModel);
            
            // MyDebug.Log($"EnemyModel {m.name} find Enter Drag");
            enemyModel.OnPointerEnterEvt += () =>
            {
                // MyDebug.Log($"EnemyModel {m.name} OnPointerEnterEvt try...");
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out var yieldCardData))
                    return;
                if (!yieldCardData.CardData.HasTarget)
                    return;
                yieldCardData.CardData.Target = enemyData;
                yieldCardData.CardModel.RefreshTxtDes();
                // MyDebug.Log($"EnemyModel {m.name} OnPointerEnterEvt success!");
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(false);
                enemyModel.EnableSelectTarget(true);
                enteredTargetEnemyModels.Add(enemyModel);
            };

            enemyModel.OnPointerExitEvt += () =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag, out var yieldCardData))
                    return;
                yieldCardData.CardData.Target = null;
                yieldCardData.CardModel.RefreshTxtDes();
                enteredTargetEnemyModels.Remove(enemyModel);
                enemyModel.EnableSelectTarget(false);
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(true);
            };
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
        MyFSM.OnRegister(BothTurnStateWrap.One, alwaysBind: BindBothTurn);
        MyFSM.OnRegister(YieldCardStateWrap.One, alwaysBind: BindYieldCard);
    }

    static bool PosInRect(Vector2 pos, RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, Camera.main);
    }
}