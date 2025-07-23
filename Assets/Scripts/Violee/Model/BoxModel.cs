using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
        [SerializeField]
        BoxData boxData;

        #region Drag In
        public List<WallModel> WallListIns;
        [SerializeField]
        SerializableDictionary<EBoxDir, BoxPointModel> pointDic;
        readonly MyKeyedCollection<EWallType, WallModel> wallKList = new(w => w.WallData.WallType);
        HashSet<BindDataAct<bool>> visitBindSet = new ();
        #endregion
        void Awake()
        {
            wallKList.Clear();
            wallKList.AddRange(WallListIns);
        }

        public void ReadData(BoxData fBoxData)
        {
            boxData = fBoxData;
            name = $"Box {boxData.Pos2D.x} {boxData.Pos2D.y}";

            visitBindSet.ForEach(b => b.UnBind());
            visitBindSet.Clear();
            boxData.PointKList.ForEach(p =>
            {
                pointDic[p.Dir].BoxPointData = p;
                visitBindSet.Add(Binder.From(p.Visited).To(pointDic[p.Dir].gameObject.SetActive).Immediate());
            });
            boxData.OnWallDataChanged += OnWallDataChanged;
            boxData.WallKList.ForEach(OnWallDataChanged);
            
            transform.position = BoxHelper.Pos2DTo3DBox(boxData.Pos2D);
        }

        void OnWallDataChanged(WallData wallData)
        {
            wallKList[wallData.WallType].ReadData(wallData);
        }
    }
}