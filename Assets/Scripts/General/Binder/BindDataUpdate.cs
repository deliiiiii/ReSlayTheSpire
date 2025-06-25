using System;

public class BindDataUpdate
{
    public Action<float> Act;
    readonly EUpdatePri priority;

    public BindDataUpdate(Action<float> act, EUpdatePri priority)
    {
        this.Act = act;
        this.priority = priority;
    }

    public void UnBind()
    {
        Updater.UpdateDic[priority].Remove(this);
    }
}

public enum EUpdatePri
{
    MainModel = 0,
    // P1 = 1,
    // P2 = 2,
}