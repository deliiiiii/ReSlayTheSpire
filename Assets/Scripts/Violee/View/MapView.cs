using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class MapView : MonoBehaviour
{
#pragma warning disable CS8618
    public Text CostTxtPrefab;
#pragma warning restore CS8618
    
    void Awake()
    {
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform);
        if (Configer.SettingsConfig.ShowBoxCost)
        {
            BoxModelManager.OnBeginDij += BindAllCostTxt;
        }
    }
    
    static readonly Dictionary<BoxPointData, (BindDataAct<int>, Text)> costTxtDic = new ();
    static ObjectPool<Text> costTxtPool = null!;
    static void DestroyAllCostTxt()
    {
        foreach (var pair in costTxtDic.Values)
        {
            costTxtPool.MyDestroy(pair.Item2);
        }
    }
    static async Task BindAllCostTxt()
    {
        try
        {
            DestroyAllCostTxt();
            foreach (var point in BoxModelManager.GetAllPoints())
            {
                var txt = await costTxtPool.MyInstantiate(point.Pos3D + Vector3.up * 0.1f);
                txt.gameObject.SetActive(true);
                var b = Binder.From(point.CostWall).To(v =>
                {
                    txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                });
                b.Immediate();
                costTxtDic.Add(point, (b, txt));
                await Configer.SettingsConfig.YieldFrames(multi : 1 / 4f);
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
}