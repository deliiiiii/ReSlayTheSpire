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
        if(Updater.UpdateDic.TryGetValue(priority, out var value))
            value.Remove(this);
    }
}

public enum EUpdatePri
{
    Default = -1,
    MainModel = 0,
    // P1 = 1,
    // P2 = 2,
}