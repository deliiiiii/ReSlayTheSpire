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
        public Transform SceneItemParent = null!;
        
        protected override void OnReadData()
        {
            name = $"Box {Data.Pos2D.x} {Data.Pos2D.y}";
            
            transform.position = BoxHelper.Pos2DTo3DBox(Data.Pos2D);
            
            wallDic.Values.ForEach(g => g.gameObject.SetActive(false));
            Data.PointDataMyDic.ForEach(pair => pointDic[pair.Key].ReadData(pair.Value));
            
            Data.WallDataMyDic.Values.ForEach(OnAddWallData);
            Data.WallDataMyDic.OnAdd += OnAddWallData;
            Data.WallDataMyDic.OnRemove += OnRemoveWallData;

            SceneItemParent.ClearChildren();
            Data.SceneDataMyList.ForEach(OnAddSceneItemData);
            Data.SceneDataMyList.OnAdd += OnAddSceneItemData;
            Data.SceneDataMyList.OnRemove += OnRemoveSceneItemData;
        }

        public void OnAddWallData(WallData wallData)
        {
            wallDic[wallData.WallType].ReadData(wallData);
        }

        public void OnRemoveWallData(WallData wallData)
        {
            wallDic[wallData.WallType].gameObject.SetActive(false);
        }

        #region SceneItem
        
        void OnAddSceneItemData(SceneItemData fdata)
        {
            if (fdata.InsModel == null)
                fdata.InsModel = Instantiate(fdata.OriginModel);
            var model = fdata.InsModel;
            model.ReadData(fdata);
            var dtRot = fdata.OccupyDirSet.First() switch
            {
                EBoxDir.Up => Quaternion.Euler(0, 0, 0),
                EBoxDir.Right => Quaternion.Euler(0, 90, 0),
                EBoxDir.Down => Quaternion.Euler(0, 180, 0),
                _ => Quaternion.Euler(0, 270, 0),
            };
            model.transform.localPosition = dtRot * model.transform.localPosition;
            model.transform.localRotation *= dtRot;
            model.transform.position += transform.position;
            model.transform.parent = SceneItemParent;
            model.gameObject.SetActive(true);
        }
        void OnRemoveSceneItemData(SceneItemData data)
        {
            Destroy(data.InsModel);
        }
        #endregion
    }
}