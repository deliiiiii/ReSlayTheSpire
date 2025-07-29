using System.Collections.Generic;
using System.Linq;
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
        foreach (var bindDataUpdate in UpdateDic.SelectMany(pair => pair.Value))
        {
            if(bindDataUpdate.GuardSet.All(guard => guard()))
                bindDataUpdate.Act(Time.deltaTime);
        }
    }
}

