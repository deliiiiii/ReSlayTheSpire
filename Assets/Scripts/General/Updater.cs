using System.Collections.Generic;
using UnityEngine;

public class Updater : Singleton<Updater>
{
    readonly SortedDictionary<EUpdatePri, HashSet<BindDataUpdate>> updateDic = new();
    public static SortedDictionary<EUpdatePri, HashSet<BindDataUpdate>> UpdateDic => Instance.updateDic;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        foreach (var pair in UpdateDic)
        {
            foreach (var bindDataUpdate in pair.Value)
            {
                bindDataUpdate.Act(Time.deltaTime);
            }
        }
    }
}

