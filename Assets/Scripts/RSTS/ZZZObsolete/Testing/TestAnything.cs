using RSTS;
using RSTS.CDMV;
using Sirenix.OdinInspector;
using UnityEngine;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

public class TestAnything : Singleton<TestAnything>
{
    [SerializeReference]
    public GameData? GameData;
    [Button]
    public void ChangePlayerHP(int delta)
    {
        GameData ??= RefPoolSingle<GameData>.Acquire();
        GameData.BattleData.CurHP.Value += delta;
    }
}