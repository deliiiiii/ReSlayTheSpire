using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;
[DefaultExecutionOrder(0)]
public class Launcher : Singleton<Launcher>, IHasBind
{
    protected override void Awake()
    {
        base.Awake();
        MyFSM.Register<EGameState>();
    }

    void Start()
    {
        OnInitAllAsync?.Invoke();
        MyFSM.EnterState(EGameState.Title);
    }

    void OnDestroy()
    {
        OnUnInitAllAsync?.Invoke();
        MyFSM.Release<EGameState>();
    }

    public static event Func<Task>? OnInitAllAsync;
    public static event Func<Task>? OnUnInitAllAsync;

    public IEnumerable<BindDataBase> GetAllBinders()
    {
#if UNITY_EDITOR
        // yield return Binder.From(_ => Sirenix.Utilities.Editor.GUIHelper.RequestRepaint());
#endif
        foreach (var b in MyFSM.GetAllBinders<EGameState>())
            yield return b;
    }
}

public enum EGameState
{
    ChoosePlayer,
    Title,
    Battle,
}

