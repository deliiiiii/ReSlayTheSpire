using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
        [ShowInInspector]
        BoxData boxData;

        public SerializableDictionary<EWallType, WallModel> WallDic;

        public void InitData(BoxData fBoxData)
        {
            boxData = fBoxData;
            boxData.OnAddWall += wallType => SetWallActive(wallType, true);
            boxData.OnRemoveWall += wallType => SetWallActive(wallType, false);
            name = $"Box {fBoxData.Pos.x} {fBoxData.Pos.y}";
            WallDic?.Keys.ForEach(wallType =>
            {
                if (fBoxData.HasWallByType(wallType))
                {
                    SetWallActive(wallType, true);
                }
            });
            
        }

        void SetWallActive(EWallType wallType, bool isActive)
        {
            WallDic[wallType].gameObject.SetActive(isActive);
        }
    }
}