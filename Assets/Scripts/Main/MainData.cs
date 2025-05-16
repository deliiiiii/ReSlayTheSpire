using System;

public enum EJobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}

[Serializable] 
public class MainData
{
    dynamic i;
    public string PlayerName;
    public Observable<float> PlayTime;
    public Observable<float> SaveTimer;
    public Observable<EJobType> PlayerJob;
} 