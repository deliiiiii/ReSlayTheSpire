using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public interface IMyFSMArg
{
    public void Launch();
    public void UnInit();
}

public interface IStateWrap
{
    public IStateWrap Parent { get; set; }
}

[Serializable]
public class StateWrap<TEnum, TArg> : StateForView<TEnum, TArg>, IStateWrap
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg
{
    [field: AllowNull, MaybeNull]
    public static StateWrap<TEnum, TArg> One => field ??= new();
    protected StateWrap()
    {
        Arg = null!;
        Fsm = null!;
        SelfTick = null!;
        Parent = null!;
    }
    internal TArg Arg;
    internal MyFSM<TEnum> Fsm;
    internal BindDataUpdate SelfTick;
    public IStateWrap Parent { get; set; }
}

[Serializable]
public class StateForView<TEnum, TArg>
    where TEnum : struct, Enum
    where TArg : class, IMyFSMArg
{
    // [field: AllowNull, MaybeNull]
    // public static StateForView<TEnum, TArg> One => field ??= new StateForView<TEnum, TArg>();
    // bindAlwaysDic包括了State的OnEnter、OnExit，和Data的事件绑定回调，如data.OnXXXEvent += () => {}
    internal List<Action<TArg>> BindAlways = [];
    // unbindDic包括BindDataBase的子类的Bind/UnBind，如进入状态时绑定UI按钮且退出状态解绑
    internal List<Func<TArg, IEnumerable<BindDataBase>>> CanUnBind = [];
    // tickDic包括自己绑定的与Data有关的Tick委托（类型Action<float, IMyFSMArg>）
    internal List<BindDataUpdate> Tick = [];
}