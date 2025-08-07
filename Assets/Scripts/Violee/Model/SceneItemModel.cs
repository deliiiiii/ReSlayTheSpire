using UnityEngine;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    [SerializeReference] public required InteractReceiver Ir;
    

    protected override void OnReadData()
    {
        if (Data.HasCount)
        {
            Data.OnRunOut += () =>
            {
                Data.HideAfterRunOut?.SetActive(false);
                Data.ShowAfterRunOut?.SetActive(true);
            };
        }

        Ir.GetInteractInfo = GetCb;
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


