using System;

namespace Violee
{
    public enum EDoorType
    {
        Random,
        None,
        Wooden,
    }
    [Serializable]
    public class WallData
    {
        WallData() { }

        public static WallData Create(EBoxDir dir, EDoorType doorType)
            => Create(BoxHelper.WallDirToType(dir), doorType);
        public static WallData Create(EWallType wallType, EDoorType doorType)
        {
            return new WallData()
            {
                WallType = wallType,
                DoorType = doorType == EDoorType.Random ? RandomDoor() : doorType
            };
        }

        static EDoorType RandomDoor()
        {
            var ran = UnityEngine.Random.value;
            return ran <= Configer.BoxConfig.DoorPossibility ? EDoorType.Wooden : EDoorType.None;
        }
        
        public EWallType WallType;
        public EDoorType DoorType;
    }
    
}