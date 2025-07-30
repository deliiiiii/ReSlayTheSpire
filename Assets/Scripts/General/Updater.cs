using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Updater : Singleton<Updater>
{
    readonly SortedDictionary<EUpdatePri, HashSet<BindDataUpdate>> updateDic = new();
    public static SortedDictionary<EUpdatePri, HashSet<BindDataUpdate>> UpdateDic => Instance.updateDic;
    void Update()
    {
        var updateDicValues = UpdateDic.Values;
        for (var i = 0; i < updateDicValues.Count; i++)
        {
            var bindDataUpdates = updateDicValues.ElementAt(i);
            var bindDataUpdatesList = bindDataUpdates.ToList();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var j = 0; j < bindDataUpdatesList.Count; j++)
            {
                var bindDataUpdate = bindDataUpdatesList[j];
                if (bindDataUpdate.GuardSet.All(guard => guard()))
                    bindDataUpdate.Act(Time.deltaTime);
            }
        }
    }
}

