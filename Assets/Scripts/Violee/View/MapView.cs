using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Violee.View
{
    public class MapView : MonoBehaviour
    {
        Dictionary<BoxPointData,(BindDataAct<int>, Text)> costTxtDic;
        public Text CostTxtPrefab;
        ObjectPool<Text> costTxtPool;

        void Awake()
        {
            costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform);
            if (Configer.SettingsConfig.ShowBoxCost)
            {
                MapModel.OnBeginDij += BindAllCostTxt;
            }
        }

        async Task DestroyAllCostTxt()
        {
            if (costTxtDic != null)
            {
                foreach (var pair in costTxtDic.Values)
                {
                    pair.Item1.UnBind();
                    costTxtPool.MyDestroy(pair.Item2);
                    await Configer.SettingsConfig.YieldFrames(multi : 1 / 10f);
                }
            }
        }
        async Task BindAllCostTxt()
        {
            try
            {
                await DestroyAllCostTxt();
                costTxtDic = new();
                foreach (var point in MapModel.GetAllPoints())
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
}