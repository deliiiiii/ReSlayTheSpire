using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RSTS;
[DefaultExecutionOrder(0)]
public class Launcher : Singleton<Launcher>, IHasBind
{
    protected override void Awake()
    {
        base.Awake();
        MyDebug.Log("Launcher Awake Register");
        MyFSM.Register<EGameState>();
        OnEnableAsync += () =>
        {
            MyDebug.Log("Launcher Enable 1 Bind");
            this.BindAll();
            MyDebug.Log("Launcher Enable 2 Enter");
            return Task.CompletedTask;
        };
        OnDisableAsync += () =>
        {
            this.UnBindAll();
            return Task.CompletedTask;
        };
    }

    void Start()
    {
        MyFSM.EnterState(EGameState.Title);
    }

    void OnDestroy()
    {
        MyFSM.Release<EGameState>();
    }

    public IEnumerable<BindDataBase> GetAllBinders()
    {
#if UNITY_EDITOR
        // yield return Binder.From(_ => Sirenix.Utilities.Editor.GUIHelper.RequestRepaint());
#endif
        foreach (var b in MyFSM.GetAllBinders<EGameState>())
            yield return b;
    }
}

public enum EGameState : long
{
    ChoosePlayer,
    Title,
    Battle,
}

