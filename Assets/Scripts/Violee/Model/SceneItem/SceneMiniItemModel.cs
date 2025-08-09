using System;
using UnityEngine;

namespace Violee;

[RequireComponent(typeof(InteractReceiver))]
public class SceneMiniItemModel : ModelBase<SceneMiniItemData>, IHasInteractReceiver
{
    void Awake()
    {
        ReadData(Data);
    }

    protected override void OnReadData()
    {
        Data.CheckData();
        var ir = GetComponent<InteractReceiver>();
        ir.GetInteractInfo = GetCb;
    }

    public InteractInfo GetCb()
    {
        return new SceneMiniItemInteractInfo
        {
            SceneMiniItemData = Data,
            Description = Data.GetInteractDes(),
            Color = Data.InteractColor,
            Act = Data.PickUp
        };
    }
}