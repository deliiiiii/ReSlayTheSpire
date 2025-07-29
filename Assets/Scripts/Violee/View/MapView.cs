using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class MapView : ViewBase
{
    public Text CostTxtPrefab = null!;
    
    protected override void IBL()
    {
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform, 42);
        if (Configer.SettingsConfig.ShowBoxCost)
            BoxModelManager.DijkstraStream.OnBeginAsync += BindAllCostTxt;
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
    static async Task BindAllCostTxt((MyKeyedCollection<Vector2Int, BoxData>, HashSet<Vector2Int>) pair)
    {
        try
        {
            await DestroyAllCostTxt();
            var boxKList = pair.Item1;
            var allPoints = boxKList.SelectMany(x => x.PointKList).ToList();
            foreach (var point in allPoints)
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