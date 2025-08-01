using System;
using UnityEngine;
using UnityEngine.Events;
using Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    InteractReceiver? ir;
    protected override void OnReadData()
    {
        ir ??= gameObject.GetComponentInChildren<InteractReceiver>();
        ir.OnEnterInteract += () => PlayerManager.GetReticleCb.Value = GetCb;
        ir.OnExitInteract += () => PlayerManager.GetReticleCb.Value = () => null;
        PlayerManager.OnClickReticle += () => PlayerManager.GetReticleCb.Value = GetCb;
    }

    SceneItemCb? GetCb()
    {
        var ret = new SceneItemCb
        {
            Des = data.GetDes(),
            Color = data.DesColor(),
            Cb = data switch
            {
                PurpleSceneItemData { Count: > 0 } pData => () => 
                {
                    PlayerManager.AddEnergy(pData.Energy);
                    pData.Count--;
                },
                _ => null!,
            }
        };
        return ret.Cb == null! ? null : ret;
    }
}

public class SceneItemCb
{
    public required string Des;
    public required Color Color;
    public required Action Cb;
}
