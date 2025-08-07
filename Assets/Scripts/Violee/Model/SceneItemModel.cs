using System.Collections.Generic;
using UnityEngine;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    [SerializeReference] public required List<InteractReceiver> IrList;
    

    protected override void OnReadData()
    {
        Data.CheckData();
        IrList.ForEach(i => i.GetInteractInfo = GetCb);
    }

    SceneItemInteractInfo? GetCb()
    {
        if (!Data.CanUse())
            return null;
        var ret = new SceneItemInteractInfo
        {
            Act = Data.Use,
            Description = Data.GetInteractDes(), 
            Color = Data.DesColor(),
            
            SceneItemData = Data,
        };
        return ret;
    }
}


