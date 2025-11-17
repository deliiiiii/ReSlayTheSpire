using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.Utilities;

public abstract class MyFSM
{
    static readonly Dictionary<Type, StateWrap> wrapDic = [];
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    static readonly Dictionary<Type, List<Action<IMyFSMArg>>> bindAlwaysDic = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    static readonly Dictionary<Type, List<Func<IMyFSMArg, IEnumerable<BindDataBase>>>> unbindDic = [];
    // tickDic包括状态机的Update调用的OnUpdate，和自己绑定的与Data有关的Tick委托（类型Action<float, IMyFSMArg>）
    static readonly Dictionary<Type, List<BindDataUpdate>> tickDic = [];
    
    public static void Register<TEnum, TArg>(StateWrap<TEnum, TArg> one, TEnum state, Func<MyFSM<TEnum>, TArg> argCtor)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        var type = one.GetType();
        var wrap = Get(one);
        if (wrap != null)
        {
            MyDebug.LogError($"Wrap {type.Name} Duplicated");
            return;
        }
        // 【0】添加Wrap
        wrapDic.Add(type, one);
        wrap = one;
        var fsm = new MyFSM<TEnum>();
        wrap.Fsm = fsm; 
        // 【1】IBL的Init，写在构造函数里了。
        var arg = argCtor(fsm);
        wrap.Arg = arg;
        // 【1.1】绑定自身Tick
        var selfTick = Binder.FromTick(dt => fsm.Update(dt), EUpdatePri.Fsm);
        wrap.SelfTick = selfTick;
        
        // 【2】IBL的Bind
        if (unbindDic.TryGetValue(type, out var unbindFuncList)) 
            unbindFuncList?.ForEach(func =>func.Invoke(arg).ForEach(bindDataBase => bindDataBase.Bind()));
        if (bindAlwaysDic.TryGetValue(type, out var bindAlwaysActList)) 
            bindAlwaysActList.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(arg));
        // 【3】IBL的Launch
        arg.Launch();
        if(tickDic.TryGetValue(type, out var tickActList))
            tickActList.ForEach(bindDataUpdate => bindDataUpdate.Bind());
        // 【4】进入初始状态
        EnterState(one, state);
    }
    
    public static void OnRegister<TEnum, TArg>(
        StateWrap<TEnum, TArg> one,
        Action<MyFSM<TEnum>, TArg>? alwaysBind = null,
        Func<TArg, IEnumerable<BindDataBase>>? canUnbind = null,
        Action<float, TArg>? tick = null)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        var type = one.GetType();
        unbindDic.TryAdd(type, []);
        if(canUnbind != null)
            unbindDic[type].Add(arg => canUnbind((TArg)arg));
        bindAlwaysDic.TryAdd(type, []);
        if(alwaysBind != null)
            bindAlwaysDic[type].Add(arg => alwaysBind.Invoke(Get(one)?.Fsm!, (TArg)arg));
        tickDic.TryAdd(type, []);
        if(tick != null)
            tickDic[type].Add(Binder.FromTick(dt => tick(dt, Get(one)?.Arg!), EUpdatePri.Fsm));
    }

    public static void Release<TEnum, TArg>(StateWrap<TEnum, TArg> one)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        if (!TryGet(one, nameof(Release), out var wrap))
            return;
        var type = one.GetType();
        // 【4】跳转到空状态
        wrap.Fsm.OnDestroy();
        // 【3】Launch的反向
        wrap.Arg.UnInit();
        tickDic[type].ForEach(bindDataUpdate => bindDataUpdate.UnBind());
        // 【2】Bind的反向
        unbindDic[type]?.ForEach(func => func.Invoke(wrap.Arg).ForEach(bindDataBase => bindDataBase.UnBind()));
        // 【1.1】自身Tick的反向。【1】构造函数不用反向。
        wrap.SelfTick.UnBind();
        // 移除Wrap
        wrapDic.Remove(type);
    }

    public static void EnterState<TEnum, TArg>(StateWrap<TEnum, TArg> one, TEnum state)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        if (!TryGet(one, nameof(EnterState), out var wrap))
            return;
        wrap.Fsm.ChangeState(state);
    }

    public static bool IsState<TEnum, TArg>(StateWrap<TEnum, TArg> one, TEnum state)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
        => Get(one)?.Fsm.IsOneOfState(state) ?? false;

    public static bool IsState<TEnum, TArg>(StateWrap<TEnum, TArg> one, TEnum state, out TArg arg)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        var wrap = Get(one);
        var ret = wrap?.Fsm.IsOneOfState(state) ?? false;
        arg = ret ? wrap?.Arg ?? null! : null!;
        return ret;
    }

    public static string ShowState<TEnum, TArg>(StateWrap<TEnum, TArg> one)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        return Get(one)?.Fsm.CurStateName ?? "Null";
    }

    static bool TryGet<TEnum, TArg>(StateWrap<TEnum, TArg> one, string log, out StateWrap<TEnum, TArg> wrap) 
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        wrap = Get(one)!;
        if (wrap != null)
            return true;
        MyDebug.LogError($"TryGet wrap {one.GetType().Name} Not Exist when {log}");
        return false;
    }

    static StateWrap<TEnum, TArg>? Get<TEnum, TArg>(StateWrap<TEnum, TArg> one)
        where TEnum : struct, Enum
        where TArg : class, IMyFSMArg
    {
        if (wrapDic.TryGetValue(one.GetType(), out var wrap))
            return wrap as StateWrap<TEnum, TArg>;
        return null;
    }

}

[Serializable]
public class MyFSM<TEnum> : MyFSM 
    where TEnum : struct, Enum
{ 
    public MyFSM()
    {
        stateDic = new Dictionary<TEnum, MyState>();
        foreach (var e in Enum.GetValues(typeof(TEnum))) 
            stateDic.Add((TEnum)e, new MyState());
    }

    public string CurStateName => curState?.ToString() ?? "Null";
    Dictionary<TEnum, MyState> stateDic;
    MyState? curStateClass;
    Enum? curState;
    
    public MyState GetState(TEnum e)
    {
        // if (e == null)
        //     return null;
        if (stateDic.TryGetValue(e, out var value))
            return value;
        MyState state = new();
        stateDic.Add(e, state);
        return state;
    }
    
    public void Update(float dt) => curStateClass?.Update(dt);

    public void ChangeState(TEnum e)
    {
        if (curStateClass == null)
        {
            Launch(e);
            return;
        }
        var newStateClass = GetState(e);
        if (newStateClass == curStateClass)
            return;
        curStateClass.Exit();
        curStateClass = newStateClass;
        curState = e;
        curStateClass.Enter();
    }

    public bool IsOneOfState(params Enum[] enums) => enums.Contains(curState);
    void Launch(TEnum startState)
    {
        curStateClass = GetState(startState);
        curState = startState;
        curStateClass.Enter();
    }

    public void OnDestroy()
    {
        curStateClass?.Exit();
        curState = null;
        curStateClass = null;
    }
}