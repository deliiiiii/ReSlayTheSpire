using System;
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

    Action? GetCb()
    {
        return data switch
        {
            PurpleSceneItemData { Count: > 0 } pData => () =>
            {
                PlayerManager.AddEnergy(pData.Energy);
                pData.Count--;
            },
            _ => null,
        };
    }
}