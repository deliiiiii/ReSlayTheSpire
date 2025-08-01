using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class MapView : ViewBase<MapView>
{
    public Text CostTxtPrefab = null!;
    protected override void IBL()
    {
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform, 42);
        if (Configer.SettingsConfig.ShowBoxCost)
            MapManager.DijkstraStream.OnBeginAsync(BindAllCostTxt);
    }
    HashSet<Text> costTxtSet = [];
    ObjectPool<Text> costTxtPool = null!;
    async Task DestroyAllCostTxt()
    {
        foreach (var text in costTxtSet)
        {
            costTxtPool.MyDestroy(text);
            await Configer.SettingsConfig.YieldFrames(multi : 1 / 16f);
        }
    }
    async Task BindAllCostTxt((GenerateStreamParam, Vector3) pair)
    {
        try
        {
            await DestroyAllCostTxt();
            var boxKList = pair.Item1.BoxKList;
            var allPoints = boxKList.SelectMany(x => x.PointDataMyDic).ToList();
            foreach (var point in allPoints)
            {
                var txt = await costTxtPool.MyInstantiate(point.Pos3D + Vector3.up * 0.1f);
                txt.gameObject.SetActive(true);
                var b = Binder.From(point.CostWall).To(v =>
                {
                    txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                });
                b.Immediate();
                costTxtSet.Add(txt);
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