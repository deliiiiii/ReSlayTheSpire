using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
        [SerializeField]
        BoxData boxData;

        #region Drag In
        public SerializableDictionary<EWallType, WallModel> WallDic;
        #endregion

        public void ReadData(BoxData fBoxData)
        {
            boxData = fBoxData;
            boxData.OnAddWall += (wallType, wallData) => SetWallActive(wallType, true, wallData.DoorType);
            boxData.OnRemoveWall += wallType => SetWallActive(wallType, false, EDoorType.None);
            name = $"Box {fBoxData.Pos.x} {fBoxData.Pos.y}";
            WallDic?.Keys.ForEach(wallType =>
            {
                if (fBoxData.HasWallByType(wallType))
                {
                    SetWallActive(wallType, true, fBoxData.WallDic[wallType].DoorType);
                }
            });
        }

        void SetWallActive(EWallType wallType, bool isActive, EDoorType doorType)
        {
            WallDic[wallType].gameObject.SetActive(isActive);
            WallDic[wallType].SetIsDoor(doorType);
        }
    }
}