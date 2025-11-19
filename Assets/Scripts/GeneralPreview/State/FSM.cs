using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public abstract class FSM<TArg, TEnum>
    where TArg : FSM<TArg, TEnum>
    where TEnum : struct, Enum
{
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    [JsonIgnore] protected static readonly List<Action<TArg, StateFunc<TEnum>>> AlwaysBindList = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    [JsonIgnore] protected static readonly List<Func<TArg, IEnumerable<BindDataBase>>> 
        CanUnbindList = [];
    public static void OnRegister(
        Action<TArg, StateFunc<TEnum>>? alwaysBind = null,
        Func<TArg, IEnumerable<BindDataBase>>? canUnbind = null)
    {
        if(canUnbind != null)
            CanUnbindList.Add(canUnbind);
        if(alwaysBind != null)
            AlwaysBindList.Add(alwaysBind.Invoke);
    }

    static FSM()
    {
        InitSubData();
    }
    protected FSM()
    {
        selfTick = Binder.FromTick(Tick, EUpdatePri.Fsm);
    }
    [HideInInspector] bool isLaunched;
    [ReadOnly] TEnum curState;
    [JsonIgnore][HideInInspector] MyState? curStateClass;
    [JsonIgnore][HideInInspector] readonly Dictionary<TEnum, MyState> stateDic = [];
    [JsonIgnore][HideInInspector] readonly BindDataUpdate selfTick;
    [JsonIgnore][HideInInspector] readonly List<BindDataBase> unbindableInstances = [];
    [HideInInspector] TArg Arg => (this as TArg)!;

    [HideInInspector] public string CurStateName => curState.ToString();
    public void EnterState(TEnum e)
    {
        if (curStateClass == null)
        {
            MyDebug.LogError("FSM Not Launched");
            return;
        }
        var newStateClass = GetStateInternal(e);
        if (newStateClass == curStateClass)
            return;
        curStateClass.Exit();
        curStateClass = newStateClass;
        curState = e;
        curStateClass.Enter();
    }
    public bool IsState(TEnum e) => IsOneOfState(e);
    public void Launch(TEnum startState)
    {
        if (isLaunched)
        {
            MyDebug.LogError($"Launch FSM: {GetType().Name} Duplicated");
            return;
        }
        // 【0】添加FSM
        isLaunched = true;
        // 【1】IBL的Init, 构造函数
        // 【2】IBL的Bind
        Bind();
        unbindableInstances.Clear();
        CanUnbindList.ForEach(func =>
        {
            func.Invoke(Arg).ForEach(bdb =>
            {
                bdb.Bind();
                unbindableInstances.Add(bdb);
            });
        });
        AlwaysBindList.ForEach(bindAlwaysAct => bindAlwaysAct.Invoke(Arg, GetState));
        selfTick.Bind();
        // 【3】IBL的Launch
        Launch();
        // 【4】进入初始状态
        curStateClass = GetState(startState);
        curState = startState;
        curStateClass.Enter();
    }
    public void Release()
    {
        if (!isLaunched)
        {
            MyDebug.LogError($"Release FSM: {GetType().Name} Not Exist");
            return;
        }
        // 【4】跳转到空状态，并清空所有状态类
        curStateClass?.Exit();
        stateDic.Clear();
        curStateClass = null;
        // 【3】Launch的反向
        UnInit();
        // 【2】Bind的反向
        selfTick.UnBind();
        unbindableInstances.ForEach(instance => instance.UnBind());
        unbindableInstances.Clear();
        // 【1】Init的反向, 无需析构函数
        // 【0】移除FSM
        isLaunched = false;
    }
    
    
    protected abstract void Bind();
    protected abstract void Launch();
    protected abstract void UnInit();

    protected virtual void Tick(float dt)
    {
        curStateClass?.Update(dt);
    }
    protected MyState GetState(TEnum e) => GetStateInternal(e);
    
    MyState GetStateInternal(TEnum e)
    {
        if (stateDic.TryGetValue(e, out var value))
            return value;
        MyState state = new();
        stateDic.Add(e, state);
        return state;
    }
    bool IsOneOfState(params TEnum[] enums) => enums.Contains(curState);

    public bool IsSubState<TSubData>([NotNullWhen(true)] out TSubData? data) 
        where TSubData : class, IBelong<TArg>
    {
        var fieldInfo = subDataDic.Keys
            .FirstOrDefault(fieldInfo => fieldInfo.FieldType == typeof(TSubData));
        if(fieldInfo != null)
        {
            data = fieldInfo.GetValue(this) as TSubData;
            return IsState(subDataDic[fieldInfo]);
        }
        MyDebug.LogError($"FSM {typeof(TArg).Name} 中不存在类型为 {typeof(TSubData).Name} 的子状态数据.");
        data = null;
        return false;
    }

    static readonly Dictionary<FieldInfo, TEnum> subDataDic = [];

    static void InitSubData()
    {
        typeof(TArg)
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .ForEach(fieldInfo =>
            {
                var attr = fieldInfo.GetCustomAttribute<SubStateAttribute<TEnum>>();
                if (attr == null)
                    return;
                if (!fieldInfo.IsPrivate)
                {
                    MyDebug.LogError($"封装性考虑, {typeof(TArg).Name}类中的字段{fieldInfo.Name}推荐为private. " +
                                     $"欲访问此字段, 请使用IsSubState<TSubData>(out TSubData data)方法.");
                }
                subDataDic.Add(fieldInfo, attr.Value);
            });
    }
}


public abstract class FSM<TArg, TEnum, TParentStateData>(TParentStateData parent)
    : FSM<TArg, TEnum>, IBelong<TParentStateData>
    where TArg : FSM<TArg, TEnum, TParentStateData>
    where TEnum : struct, Enum
{
    [HideInInspector] public TParentStateData Parent { get; } = parent;
}

public interface IBelong<out T>
{
    [HideInInspector] public T Parent { get; }
}
public delegate MyStateForView StateFunc<in TEnum>(TEnum e) where TEnum : struct, Enum;


[AttributeUsage(AttributeTargets.Field)]
public class SubStateAttribute<TEnum>(TEnum value) : Attribute
    where TEnum : struct, Enum
{
    public readonly TEnum Value = value;
}