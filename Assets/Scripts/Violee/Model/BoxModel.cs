using System;
using Sirenix.Utilities;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Violee
{
    public class BoxModel : ModelBase<BoxData>
    {
        public required SerializableDictionary<EWallType, WallModel> wallDic;
        public required SerializableDictionary<EBoxDir, BoxPointModel> pointDic;
        
        [NonSerialized] readonly SerializableDictionary<SceneItemData, SceneItemModel> sceneItemDic = [];
        
        protected override void OnReadData()
        {
            name = $"Box {Data.Pos2D.x} {Data.Pos2D.y}";
            transform.position = BoxHelper.Pos2DTo3DBox(Data.Pos2D);
            
            wallDic.Values.ForEach(g => g.gameObject.SetActive(false));
            Data.PointDataMyDic.ForEach(p => pointDic[p.Dir].ReadData(p));
            sceneItemDic.Values.ForEach(s => Destroy(s.gameObject));
            sceneItemDic.Clear();
            
            Data.WallDataMyDic.ForEach(OnAddWallData);
            Data.WallDataMyDic.OnAdd += OnAddWallData;
            Data.WallDataMyDic.OnRemove += OnRemoveWallData;
            
            Data.SceneDataMyList.ForEach(OnAddSceneItemData);
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
        
        void OnAddSceneItemData(SceneItemData data)
        {
            var obj = Instantiate(data.Obj);
            var model = obj.GetOrAddComponent<SceneItemModel>();
            sceneItemDic.Add(data, model);
            model.ReadData(data);
            
            var dtRot = data.OccupyDirSet.First() switch
            {
                EBoxDir.Up => Quaternion.Euler(0, 0, 0),
                EBoxDir.Right => Quaternion.Euler(0, 90, 0),
                EBoxDir.Down => Quaternion.Euler(0, 180, 0),
                _ => Quaternion.Euler(0, 270, 0),
            };
            obj.transform.localPosition = dtRot * obj.transform.localPosition;
            obj.transform.localRotation *= dtRot;
            obj.transform.parent = pointDic[data.OccupyDirSet.First()].transform;
            obj.SetActive(true);
        }
        void OnRemoveSceneItemData(SceneItemData data)
        {
            Destroy(sceneItemDic[data].gameObject);
            sceneItemDic.Remove(data);
        }
        #endregion
    }
}