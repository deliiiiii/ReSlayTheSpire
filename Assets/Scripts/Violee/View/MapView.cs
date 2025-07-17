using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View
{
    public class MapView : ViewBase
    {
        Dictionary<BoxPointData,(BindDataAct<int>, Text)> costTxtDic;
        public int TxtPerFrame = 100;
        public Text CostTxtTemplate;

        void Awake()
        {
            if (Configer.Instance.SettingsConfig.ShowBoxCost)
            {
                MapModel.OnBeginDij += BindAllCostTxt;
            }
        }

        async Task DestroyAllCostTxt()
        {
            int c = 0;
            if (costTxtDic != null)
            {
                foreach (var pair in costTxtDic.Values)
                {
                    // TODO 对象池
                    pair.Item1.UnBind();
                    Destroy(pair.Item2);
                    c++;
                    if (c >= TxtPerFrame)
                    {
                        await Task.Yield();
                        c = 0;
                    }
                }
            }
        }
        async Task BindAllCostTxt()
        {
            try
            {
                
                DestroyAllCostTxt();
                costTxtDic = new();
                int c = 0;
                foreach (var point in MapModel.GetAllPoints())
                {
                    var txt = Instantiate(CostTxtTemplate, transform);
                    txt.transform.position = BoxHelper.Pos2DTo3D(point.Pos2D) + Vector3.up * 0.1f;
                    txt.gameObject.SetActive(true);
                    var b = Binder.From(point.CostWall).To(v =>
                    {
                        txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                    });
                    b.Immediate();
                    costTxtDic.Add(point, (b, txt));
                    c++;
                    if (c >= TxtPerFrame)
                    {
                        await Task.Yield();
                        c = 0;
                    }
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