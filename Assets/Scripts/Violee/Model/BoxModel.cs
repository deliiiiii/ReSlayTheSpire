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
            transform.position = BoxHelper.Pos2DTo3DBox(data.Pos2D);
            wallDic.Values.ForEach(g => g.gameObject.SetActive(false));
            data.PointDataMyDic.ForEach(p =>
            {
                pointDic[p.Dir].ReadData(p);
            });
            data.WallDataMyDic.ForEach(OnAddWallData);
            data.OnAddWallData += OnAddWallData;
            data.OnRemoveWallData += OnRemoveWallData;
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
        public void CreateSceneItemModel(EBoxDir dir, SceneItemConfig sceneItemConfig)
        {
            var sceneItemData = SceneItemData.ReadConfig(sceneItemConfig, new([dir]));
            sceneItemData.Parent = pointDic[dir].transform;
            data.SceneDataList.MyAdd(sceneItemData);
        }
        

        #endregion
    }
}