using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class ChooseNameModel : Singleton<ChooseNameModel>, IHasBind
{
    protected override void Awake()
    {
        base.Awake();
        MyDebug.Log("ChooseNameModel Awake");
        OnEnableAsync += () =>
        {
            MyDebug.Log("ChooseNameModel Enable 1 Bind");
            this.BindAll();
            return Task.CompletedTask;
        };
        OnDisableAsync += () =>
        {
            this.UnBindAll();
            return Task.CompletedTask;
        };
    }
    public IEnumerable<BindDataBase> GetAllBinders()
    {
        yield return StateFactory.GetBindState(EGameState.ChoosePlayer)
            .OnEnter(() =>
            {
                MyDebug.Log("ChooseNameView Show");
            })
            .OnUpdate(_ =>
            {
                MyDebug.Log("ChooseNameView Update");
            });
    }
}