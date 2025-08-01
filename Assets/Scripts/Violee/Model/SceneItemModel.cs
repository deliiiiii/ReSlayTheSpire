using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    [ShowInInspector] SceneItemData shownData => data;

    public required GameObject HideAfterRunOut;
    public required GameObject ShowAfterRunOut;
    
    protected override void OnReadData()
    {
        if (data.HasCount)
        {
            data.OnRunOut += () =>
            {
                HideAfterRunOut.SetActive(false);
                ShowAfterRunOut.SetActive(true);
            };
        }
        
        var ir = gameObject.GetComponentInChildren<InteractReceiver>();
        ir.OnEnterInteract += () => PlayerManager.ReticleCb = GetCb();
        ir.OnExitInteract += () => PlayerManager.ReticleCb = null;
        PlayerManager.OnClickReticle += () => PlayerManager.ReticleCb = GetCb();
    }

    SceneItemCb? GetCb()
    {
        var ret = new SceneItemCb
        {
            Des = data.GetDes(),
            Color = data.DesColor(),
            Cb = !data.CanUse() ? null! : data switch
            {
                PurpleSceneItemData pData => () => 
                {
                    PlayerManager.AddEnergy(pData.Energy);
                    pData.Use();
                },
                _ => null!,
            }
        };
        if (ret.Cb == null!)
            return null;
        ret.Cb += () =>
        {
            PlayerManager.ReticleCb = GetCb();
        };
        return ret;
    }
}

public class SceneItemCb
{
    public required string Des;
    public required Color Color;
    public required Action Cb;
}
