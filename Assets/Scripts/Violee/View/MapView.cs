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
        Dictionary<BoxPointData, Text> costTxtDic;
        public int TxtPerFrame = 100;
        public Text CostTxtTemplate;

        void Awake()
        {
            MapModel.OnBeginDij += BindAllCostTxt;
        }
        async Task BindAllCostTxt()
        {
            try
            {
                // TODO 对象池
                costTxtDic?.Values.ForEach(Destroy);
                costTxtDic = new Dictionary<BoxPointData, Text>();
                int c = 0;
                foreach (var point in MapModel.GetAllPoints())
                {
                    var txt = Instantiate(CostTxtTemplate, transform);
                    txt.transform.position = point.Pos;
                    txt.gameObject.SetActive(true);
                    costTxtDic.Add(point, txt);
                    Binder.From(point.CostWall).To(v =>
                    {
                        txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                    }).Immediate();
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