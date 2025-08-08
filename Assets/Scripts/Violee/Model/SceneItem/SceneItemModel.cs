using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Violee;


public class SceneItemModel : ModelBase<SceneItemData>, IHasInteractReceiver
{
    [SerializeReference] public required List<InteractReceiver> IrList;
    

    protected override void OnReadData()
    {
        Data.CheckData();
        IrList.ForEach(i => i.GetInteractInfo = GetCb);
        if (Data.HasSpreadPos)
        {
            Data.HasSpreadObjList.OnAdd += model =>
            {
                model.Data.OnPickedUp += () =>
                {
                    Data.HasSpreadObjList.MyRemove(model);
                };
            };
            Data.HasSpreadObjList.OnRemove += model =>
            {
                Destroy(model.gameObject);
            };
            SpreadMiniItem();
        }
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


    void SpreadMiniItem()
    {
        int spreadCount = 0;
        foreach (var (trans, objList) in Data.SpreadObjectDic)
        {
            if(Data.HasSpreadObjList.Any(obj => obj.transform.position == trans.position))
                continue;
            if(Random.Range(0f, 1f) > Data.SpreadPossibility)
                continue;
            spreadCount++;
            var modelIns = Instantiate(objList.RandomItem(), trans);
            modelIns.ReadData(modelIns.Data);
            Data.HasSpreadObjList.MyAdd(modelIns);
            if(spreadCount >= Data.SpreadMaxCount)
                break;
        }
    }
}


