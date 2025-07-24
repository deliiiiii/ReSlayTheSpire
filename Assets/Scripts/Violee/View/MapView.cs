using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class MapView : MonoBehaviour
{
#pragma warning disable CS8618
    public Text CostTxtPrefab;
#pragma warning restore CS8618
    
    readonly Dictionary<BoxPointData, (BindDataAct<int>, Text)> costTxtDic = new ();
    ObjectPool<Text>? CostTxtPool => field ??= new ObjectPool<Text>(CostTxtPrefab, transform);

    void Awake()
    {
        if (Configer.SettingsConfig.ShowBoxCost)
        {
            MapModel.OnBeginDij += BindAllCostTxt;
        }
    }

    async Task DestroyAllCostTxt()
    {
        foreach (var pair in costTxtDic.Values)
        {
            pair.Item1.UnBind();
            CostTxtPool.MyDestroy(pair.Item2);
            await Configer.SettingsConfig.YieldFrames(multi : 1 / 10f);
        }
    }
    async Task BindAllCostTxt()
    {
        try
        {
            await DestroyAllCostTxt();
            foreach (var point in MapModel.GetAllPoints())
            {
                var txt = await CostTxtPool.MyInstantiate(point.Pos3D + Vector3.up * 0.1f);
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