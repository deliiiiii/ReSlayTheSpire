using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS.CDMV;
using UnityEngine;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

[DefaultExecutionOrder(10)]
public class BattleView : Singleton<BattleView>, IHasBind
{
    protected override void Awake()
    {
        base.Awake();
        Launcher.OnInitAllAsync += () =>
        {
            GameData = RefPool<DataBase>.RegisterOne(() => new GameData());
            this.BindAll();
            return Task.CompletedTask;
        };
    }
    
    public GameData GameData;

    public IEnumerable<BindDataBase> GetAllBinders()
    {
        yield return MyFSM.GetBindState(EGameState.Battle)
            .OnEnter(() =>
            {
            });
    }
}