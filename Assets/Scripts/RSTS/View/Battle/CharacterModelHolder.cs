using System.Collections.Generic;
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
    public bool CheckInNoTarget(Vector2 pos)
    {
        return PosInRect(pos, TransNoTargetArea.RectTransform);
    }
    public bool CheckInEnemy(Vector2 pos, int id)
    {
        if(id < 0 || id >= EnemyModelList.Count)
            return false;
        return PosInRect(pos, EnemyModelList[id].RectHolder.RectTransform);
    }

    public void BindBothTurnData(SlotDataMulti.BattleData.BothTurnData bothTurnData)
    {
        bothTurnData.EnemyList.ForEach((enemyData, i) =>
        {
            var enemyModel = Instantiate(PrbEnemy, TransEnemy[i]);
            enemyModel.ReadData(enemyData);
            enemyModel.gameObject.SetActive(true);
            EnemyModelList.Add(enemyModel);
        });
    }

    public void OnExitBothTurn()
    {
        EnemyModelList.ForEach(m => Destroy(m.gameObject));
        EnemyModelList.Clear();
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