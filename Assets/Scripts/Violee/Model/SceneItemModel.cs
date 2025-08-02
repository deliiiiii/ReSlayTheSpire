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
        
        var ir = gameObject.GetComponentInChildren<InteractReceiver>();
        ir.OnEnterInteract += () => PlayerManager.ReticleCb = GetCb();
        ir.OnExitInteract += () => PlayerManager.ReticleCb = null;
    }

    SceneItemCb? GetCb()
    {
        if (!Data.CanUse())
            return null;
        return new SceneItemCb
        {
            Data = Data,
            Cb = () => 
            {
                Data.Use();
                PlayerManager.ReticleCb = GetCb();
            },
        };
    }
}

public struct SceneItemCb
{
    public SceneItemData Data;
    public Action? Cb;
}
