using System.Collections.Generic;
using UnityEngine;

namespace Violee;


public class SceneItemModel : ModelBase<SceneItemData>, IHasInteractReceiver
{
    [SerializeReference] public required List<InteractReceiver> IrList;
    

    protected override void OnReadData()
    {
        Data.CheckData();
        IrList.ForEach(i => i.GetInteractInfo = GetCb);
    }
    
    public InteractInfo GetCb()
    {
        if (!Data.IsActive())
            return InteractInfo.CreateUnActive();
        if (!Data.CanUse(out var failReason))
        {
            return InteractInfo.CreateInvalid(failReason);
        }
        var ret = new SceneItemInteractInfo
        {
            CanUse = true,
            Act = Data.Use,
            Description = Data.GetInteractDes(), 
            Color = Data.DesColor(),
            
            SceneItemData = Data,
        };
        return ret;
    }
}


