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
            boxData.OnAddWall += wallData => SetWall(wallData, true);
            boxData.OnRemoveWall += wallData => SetWall(wallData, false);
            name = $"Box {fBoxData.Pos2D.x} {fBoxData.Pos2D.y}";

            visitBindSet.ForEach(b => b.UnBind());
            visitBindSet.Clear();
            fBoxData.PointKList.ForEach(p =>
            {
                pointDic[p.Dir].BoxPointData = p;
                visitBindSet.Add(Binder.From(p.Visited).To(pointDic[p.Dir].gameObject.SetActive).Immediate());
            });
            
            wallKList?.Select(w => w.WallData).ForEach(wallData =>
            {
                SetWall(wallData, false);
                if (fBoxData.HasWallByType(wallData.WallType, out var wallDataNew))
                {
                    SetWall(wallDataNew, true);
                }
            });
            transform.position = BoxHelper.Pos2DTo3DBox(fBoxData.Pos2D);
        }

        void SetWall(WallData wallData, bool hasWall)
        {
            wallData.HasWall = hasWall;
            wallKList[wallData.WallType].ReadData(wallData);
        }
    }
}