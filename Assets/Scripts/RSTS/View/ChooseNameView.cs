using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;

[DefaultExecutionOrder(10)]
public class ChooseNameView : Singleton<ChooseNameView>, IHasBind
{
    protected override void Awake()
    {
        base.Awake();
        Launcher.OnInitAllAsync += () =>
        {
            this.BindAll();
            return Task.CompletedTask;
        };
    }

    public IEnumerable<BindDataBase> GetAllBinders()
    {
        yield return MyFSM.GetBindState(EGameState.ChoosePlayer)
            .OnEnter(() => MyDebug.Log("ChooseNameView Show"))
            .OnUpdate(_ => MyDebug.Log("ChooseNameView Update"));
    }
}