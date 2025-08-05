using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    [SerializeReference] public required GameObject HideAfterRunOut;
    [SerializeReference] public required GameObject ShowAfterRunOut;
    [SerializeReference] public required InteractReceiver Ir;

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
            SleepTime = 2.89f,
        };
    }
}


