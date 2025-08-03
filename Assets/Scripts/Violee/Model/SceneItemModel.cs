using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    // [ShowInInspector] SceneItemData shownData => data;

    public required GameObject HideAfterRunOut;
    public required GameObject ShowAfterRunOut;
    public required InteractReceiver Ir;
    
    protected override void OnReadData()
    {
        if (Data.HasCount)
        {
            Data.OnRunOut += () =>
            {
                HideAfterRunOut.SetActive(false);
                ShowAfterRunOut.SetActive(true);
            };
        }

        Ir.GetInteractInfo = GetCb;
    }

    InteractInfo? GetCb()
    {
        if (!Data.CanUse())
            return null;
        return new InteractInfo
        {
            Act = Data.Use,
            Description = Data.GetInteractDes(), 
            Color = Data.DesColor(),
            
            IsSleep = Data is PurpleSceneItemData,
            SleepTime = Data is PurpleSceneItemData ? 2.89f : 0f,
        };
    }
}


