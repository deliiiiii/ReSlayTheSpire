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
                HasWall = true,
                HasFoundWall = new Observable<bool>(false),
                DoorType = doorType == EDoorType.Random ? RandomDoor() : doorType,
                HasFoundDoor = new Observable<bool>(false),
                Opened = false,
            };
        }

        static EDoorType RandomDoor()
        {
            var ran = UnityEngine.Random.value;
            return ran <= Configer.BoxConfig.DoorPossibility ? EDoorType.Wooden : EDoorType.None;
        }
        
        public EWallType WallType;
        public bool HasWall;
        public Observable<bool> HasFoundWall;
        public EDoorType DoorType;
        public bool HasDoor => DoorType != EDoorType.None;
        public Observable<bool> HasFoundDoor;
        public bool Opened;
    }
    
}