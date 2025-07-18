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
            boxData.OnAddWall += (wallType, wallData) => SetWall(wallType, true, wallData.DoorType);
            boxData.OnRemoveWall += wallType => SetWall(wallType, false, EDoorType.None);
            name = $"Box {fBoxData.Pos.x} {fBoxData.Pos.y}";
            WallDic?.Keys.ForEach(wallType =>
            {
                SetWall(wallType, false, EDoorType.None);
                if (fBoxData.HasWallByType(wallType))
                {
                    SetWall(wallType, true, fBoxData.WallDic[wallType].DoorType);
                }
            });
        }

        void SetWall(EWallType wallType, bool isActive, EDoorType doorType)
        {
            WallDic[wallType].gameObject.SetActive(isActive);
            WallDic[wallType].SetIsDoor(doorType);
        }
    }
}