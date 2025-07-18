using System;
using System.Collections.ObjectModel;

namespace Violee
{
    public enum EDoorType
    {
        None,
        Wooden,
    }
    [Serializable]
    public class WallData
    {
        WallData() { }

        public static WallData Create()
        {
            return new WallData() { DoorType = RandomDoor() };
        }

        static EDoorType RandomDoor()
        {
            var ran = UnityEngine.Random.value;
            return ran <= Configer.Instance.BoxConfig.DoorPossibility ? EDoorType.Wooden : EDoorType.None;
        }
        
        public EDoorType DoorType;

        public static WallData NoDoor => new WallData() { DoorType = EDoorType.None };
    }
    
}