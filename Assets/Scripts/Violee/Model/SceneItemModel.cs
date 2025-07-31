using System;
using UnityEngine.Events;
using Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    protected override void OnReadData()
    {
        gameObject.GetComponentInChildren<InteractReceiver>().OnEnterInteract += data switch
        {
            PurpleSceneItemData pData => () =>
            {
                PlayerManager.AddEnergy(pData.Energy);
                pData.Count--;
            },
            _  => null
        };
    }
}