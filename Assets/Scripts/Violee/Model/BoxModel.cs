using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : ModelBase<BoxData>
    {
#pragma warning disable CS8618
        [SerializeField] SerializableDictionary<EWallType, WallModel> wallDic;
        [SerializeField] SerializableDictionary<EBoxDir, BoxPointModel> pointDic;
#pragma warning restore CS8618
        
        protected override void OnReadData()
        {
            name = $"Box {data.Pos2D.x} {data.Pos2D.y}";
            
            data.PointKList.ForEach(p =>
            {
                pointDic[p.Dir].ReadData(p);
            });
            
            data.OnWallDataChanged += OnWallDataChanged;
            data.WallKList.ForEach(OnWallDataChanged);
            transform.position = BoxHelper.Pos2DTo3DBox(data.Pos2D);
        }

        void OnWallDataChanged(WallData wallData)
        {
            wallDic[wallData.WallType].ReadData(wallData);
        }
    }
}