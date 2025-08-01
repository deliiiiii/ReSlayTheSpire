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

        #region SceneItem
        public void CreateSceneItemModel(EBoxDir dir, SceneItemConfig sceneItemConfig)
        {
            var sceneItemData = new SceneItemData(sceneItemConfig);
            sceneItemData.OccupyDirSet = [dir];
            data.SceneItemList.Add(sceneItemData);
            var obj = Instantiate(sceneItemConfig.Object);
            var localPos = obj.transform.localPosition;
            var dtRot = dir switch
            {
                EBoxDir.Up => Quaternion.Euler(0, 0, 0),
                EBoxDir.Right => Quaternion.Euler(0, 90, 0),
                EBoxDir.Down => Quaternion.Euler(0, 180, 0),
                _ => Quaternion.Euler(0, 270, 0),
            };
            obj.transform.localPosition = dtRot * localPos;
            obj.transform.localRotation *= dtRot;
            obj.transform.parent = pointDic[dir].transform;
            obj.GetOrAddComponent<SceneItemModel>().ReadData(sceneItemData);
            obj.SetActive(true);
        }
        

        #endregion
    }
}