using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
#pragma warning disable CS8618
        #region Inspector
        [SerializeField][ReadOnly] BoxData boxData;
        [SerializeField] List<WallModel> wallListIns;
        [SerializeField] SerializableDictionary<EBoxDir, BoxPointModel> pointDic;
        #endregion
#pragma warning restore CS8618
        
        
        readonly MyKeyedCollection<EWallType, WallModel> wallKList = new(w => w.WallData.WallType);
        readonly HashSet<BindDataAct<bool>> visitBindSet = new ();
        void Awake()
        {
            wallKList.Clear();
            wallKList.AddRange(wallListIns);
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