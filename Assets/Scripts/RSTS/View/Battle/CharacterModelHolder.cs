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
    
    public bool TargetingEnemy => enteredTargetEnemyModels.Any();
    public bool CheckInNoTarget(Vector2 screenPos)
    {
        return PosInRect(screenPos, TransNoTargetArea.RectTransform);
    }

    public void BindBothTurnData(BothTurnData bothTurnData)
    {
        bothTurnData.EnemyList.OnAdd += enemyData =>
        {
            var m = Instantiate(PrbEnemy, TransEnemy[bothTurnData.EnemyList.Count - 1]);
            m.ReadData(enemyData);
            m.OnPointerEnterEvt += () =>
            {
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(false);
                m.EnableSelectTarget(true);
                enteredTargetEnemyModels.Add(m);
            };
                
            m.OnPointerExitEvt += () =>
            {
                enteredTargetEnemyModels.Remove(m);
                m.EnableSelectTarget(false);
                enteredTargetEnemyModels.LastOrDefault()?.EnableSelectTarget(true);
            };
            
            m.gameObject.SetActive(true);
            EnemyModelList.Add(m);
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

    public IEnumerable<BindDataBase> GetPlayerYieldStateBinds(BothTurnData bothTurnData)
    {
        yield return MyFSM.GetBindState(YieldCardStateWrap.One, EYieldCardState.Drag)
            .OnUpdate(_ =>
            {
                foreach (var m in EnemyModelList)
                {
                    m.CanBeSelect = bothTurnData.HasSelectTarget;
                }
            })
            .OnExit(() =>
            {
                foreach (var m in EnemyModelList)
                {
                    m.CanBeSelect = false;
                }
                foreach (var enemyModel in enteredTargetEnemyModels)
                {
                    enemyModel.EnableSelectTarget(false);
                }
                enteredTargetEnemyModels.Clear();
            });
    }
    
    public void EnableNoTargetArea(bool enable)
    {
        TransNoTargetArea.gameObject.SetActive(enable);
    }
    
    static bool PosInRect(Vector2 pos, RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, Camera.main);
    }
}