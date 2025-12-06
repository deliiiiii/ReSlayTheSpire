using System;

namespace RSTS;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
public class YieldCardData(BothTurnData parent) : FSM<YieldCardData, EYieldCardState, BothTurnData>(parent)
{
    public CardModel CardModel;
    protected override void Bind(Func<EYieldCardState, IStateForData> getState) { }
    protected override void UnInit() { }
}