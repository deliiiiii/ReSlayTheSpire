using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : ModelBase<BoxData>
    {
        public required SerializableDictionary<EWallType, WallModel> wallDic;
        public required SerializableDictionary<EBoxDir, BoxPointModel> pointDic;
        
        protected override void OnReadData()
        {
            Data.Model = this;
            
            name = $"Box {Data.Pos2D.x} {Data.Pos2D.y}";
            transform.position = BoxHelper.Pos2DTo3DBox(Data.Pos2D);
            wallDic.Values.ForEach(g => g.gameObject.SetActive(false));
            Data.PointDataMyDic.ForEach(p => pointDic[p.Dir].ReadData(p));
            Data.WallDataMyDic.ForEach(OnAddWallData);
            Data.WallDataMyDic.OnAdd += OnAddWallData;
            Data.WallDataMyDic.OnRemove += OnRemoveWallData;
            Data.SceneDataMyList.OnAdd += OnAddSceneItemData;
            Data.SceneDataMyList.OnRemove += OnRemoveSceneItemData;
        }

        public void OnAddWallData(WallData wallData)
        {
            wallDic[wallData.WallType].ReadData(wallData);
        }

        public void OnRemoveWallData(EWallType wallType)
        {
            wallDic[wallType].gameObject.SetActive(false);
        }

        #region SceneItem
        
        static void OnAddSceneItemData(SceneItemData data)
        {
            var obj = Instantiate(data.Obj);
            var localPos = obj.transform.localPosition;
            var dtRot = data.OccupyDirSet.First() switch
            {
                EBoxDir.Up => Quaternion.Euler(0, 0, 0),
                EBoxDir.Right => Quaternion.Euler(0, 90, 0),
                EBoxDir.Down => Quaternion.Euler(0, 180, 0),
                _ => Quaternion.Euler(0, 270, 0),
            };
            obj.transform.localPosition = dtRot * localPos;
            obj.transform.localRotation *= dtRot;
            obj.transform.parent = data.Parent;
            data.ObjIns = obj.GetOrAddComponent<SceneItemModel>();
            data.ObjIns.ReadData(data);
            obj.SetActive(true);
        }
        static void OnRemoveSceneItemData(SceneItemData data)
        {
            Destroy(data.ObjIns);
        }
        #endregion
    }
}