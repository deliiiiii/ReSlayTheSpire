using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;



public class CharacterModelHolder : Singleton<CharacterModelHolder>
{
    public RectHolder TransNoTargetArea;
    public GameObject PnlPlayerWarning;
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
    
    IEnumerable<BindDataBase> OnRegisterBothTurnState(BothTurnData bothTurnData)
    {
        bothTurnData.EnemyList.OnAdd += enemyData =>
        {
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
                if (!yieldCardData.HasTarget)
                    return;
                // MyDebug.Log($"EnemyModel {m.name} OnPointerEnterEvt success!");
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(false);
                m.EnableSelectTarget(true);
                enteredTargetEnemyModels.Add(m);
            };

            m.OnPointerExitEvt += () =>
            {
                if (!MyFSM.IsState(YieldCardStateWrap.One, EYieldCardState.Drag))
                    return;
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

        yield break;
    }

    void OnChangeYieldCardState(YieldCardData yieldCardData)
    {
        MyFSM.GetBindState(YieldCardStateWrap.One, EYieldCardState.Drag)
            .OnExit(() =>
            {
                EnemyModelList.ForEach(m =>
                {
                    m.EnableSelectTarget(false);
                });
                enteredTargetEnemyModels.Clear();
            });
    }
    
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(BothTurnStateWrap.One, onRegister: OnRegisterBothTurnState);
        MyFSM.OnRegister(YieldCardStateWrap.One, onChange: OnChangeYieldCardState);
    }
    
    static bool PosInRect(Vector2 pos, RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, Camera.main);
    }
}