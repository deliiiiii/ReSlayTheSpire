using System.Collections.Generic;
using RSTS.CDMV;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS;

public class CharacterModelHolder : Singleton<CharacterModelHolder>
{
    // TODO 存储敌人的矩形区域
    BothTurnData bothTurnData;
    public RectTransform TransNoTargetArea;
    public GameObject PnlPlayerWarning;
    public PlayerModel MdlPlayer;
    public HPModel PlayerHP;
    public List<EnemyModel> MdlEnemyList = [];
    public List<HPModel> EnemyHP = [];
    public bool CheckPointerPos(Vector2 pos)
    {
        var ret = RectTransformUtility.RectangleContainsScreenPoint(
            TransNoTargetArea, pos, Camera.main);
        // Debug.Log("CheckPointerPos: " + ret + pos);
        return ret;
    }

    public IEnumerable<BindDataBase> GetBattleStateBinds()
    {
        yield return MyFSM.GetBindState(EGameState.Battle)
            .OnEnter(() =>
            {
                bothTurnData = RefPoolSingle<BothTurnData>.Acquire();
            });
    }
    public void EnableNoTargetArea(bool enable)
    {
        TransNoTargetArea.gameObject.SetActive(enable);
    }
}