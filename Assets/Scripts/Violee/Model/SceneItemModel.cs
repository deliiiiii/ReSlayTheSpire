using System;
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

        Ir.InteractCb = GetCb;
    }

    InteractCb? GetCb()
    {
        return new InteractCb
        {
            Condition = Data.CanUse,
            Cb = Data.Use,
            Description = Data.GetInteractDes(),
            Color = Data.DesColor(),
        };
    }
}


