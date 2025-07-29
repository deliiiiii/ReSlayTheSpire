using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class MapView : ViewBase<BoxModelManager>
{
    public Text CostTxtPrefab = null!;
    
    

    protected override void Awake()
    {
        base.Awake();
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform, 42);
    }

    protected override void Bind()
    {
        if (Configer.SettingsConfig.ShowBoxCost)
            BoxModelManager.OnBeginDij += BindAllCostTxt;
    }


    static readonly Dictionary<BoxPointData, Text> costTxtDic = new ();
    static ObjectPool<Text> costTxtPool = null!;
    static async Task DestroyAllCostTxt()
    {
        foreach (var text in costTxtDic.Values)
        {
            costTxtPool.MyDestroy(text);
            await Configer.SettingsConfig.YieldFrames(multi : 1 / 16f);
        }
    }
    static async Task BindAllCostTxt()
    {
        try
        {
            await DestroyAllCostTxt();
            foreach (var point in BoxModelManager.GetAllPoints())
            {
                var txt = await costTxtPool.MyInstantiate(point.Pos3D + Vector3.up * 0.1f);
                txt.gameObject.SetActive(true);
                var b = Binder.From(point.CostWall).To(v =>
                {
                    txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                });
                b.Immediate();
                costTxtDic.Add(point, txt);
                await Configer.SettingsConfig.YieldFrames(multi : 1 / 8f);
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
}