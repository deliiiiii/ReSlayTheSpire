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
        [SerializeField]
        BoxData boxData;

        #region Drag In
        public List<WallModel> WallListIns;
        readonly MyKeyedCollection<EWallType, WallModel> wallKList = new(w => w.WallData.WallType);
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
            name = $"Box {fBoxData.Pos.x} {fBoxData.Pos.y}";
            wallKList?.Select(w => w.WallData).ForEach(wallData =>
            {
                SetWall(wallData, false);
                if (fBoxData.HasWallByType(wallData.WallType))
                {
                    SetWall(wallData, true);
                }
            });
        }

        void SetWall(WallData wallData, bool isActive)
        {
            var wallType = wallData.WallType;
            var doorType = wallData.DoorType;
            wallKList[wallType].gameObject.SetActive(isActive);
            wallKList[wallType].SetDoor(doorType);
        }
    }
}