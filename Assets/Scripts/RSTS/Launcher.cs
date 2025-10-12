
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace RSTS;

public class Launcher : Singleton<Launcher>, IHasBind
{
    FSMHolder<EGameState> gameFsmHolder = null!;
    FSMHolder<EState2> fsmHolder = null!;

    protected override void Awake()
    {
        base.Awake();
        OnEnableAsync += () =>
        {
            gameFsmHolder = new FSMHolder<EGameState>();
            gameFsmHolder.EnterState(EGameState.Title);
            fsmHolder = new FSMHolder<EState2>();
            fsmHolder.EnterState(EState2.B);
            
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
#if UNITY_EDITOR
        yield return Binder.From(_ => GUIHelper.RequestRepaint());
#endif
        foreach (var b in gameFsmHolder.GetAllBinders())
            yield return b;
        yield return gameFsmHolder
            .GetBindState(EGameState.Title)
            .OnUpdate(_ => MyDebug.LogWarning("Title Update"));
        yield return Binder.From(_ =>
        {
            MyDebug.LogWarning("Test.. Binder From Action<float>");
        });
    }
}

public enum EGameState
{
    Title,
    S2,
    S3,
}

public enum EState2
{
    A, B, C
}

public class FSMHolder<T> : IHasBind
    where T : Enum
{
    readonly MyFSM<T> gameFsm = new ();
    public FSMHolder()
    {
        MyDebug.Log("FSMHolder ctor" + typeof(T).Name);
    }

    // public void Dispose()
    // {
    //     MyDebug.Log("FSMHolder Dispose" + typeof(T).Name);
    // }
    ~FSMHolder()
    {
        MyDebug.Log("FSMHolder deconstruct" + typeof(T).Name);
    }

    public IEnumerable<BindDataBase> GetAllBinders()
    {
        yield return Binder.From(dt => gameFsm.Update(dt), EUpdatePri.Fsm);
    }
    

    public void EnterState(T state)
        => gameFsm.ChangeState(state);
    public bool IsState(T state)
        => gameFsm.IsState(state);

    public BindDataState GetBindState(T state) => Binder.From(gameFsm.GetState(state));
}