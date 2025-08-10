using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Violee
{
    public enum EDoorType
    {
        Random,
        None,
        Wooden,
    }
    [Serializable]
    [method: JsonConstructor]
    public class WallData(EWallType wallType, EDoorType doorType) : DataBase
    {
        public WallData(EBoxDir dir, EDoorType doorType) 
            : this(BoxHelper.WallDirToType(dir), doorType){}
        
        [NonSerialized] public required BoxData BelongBox;
        static EDoorType RandomDoor()
        {
            var ran = UnityEngine.Random.value;
            return ran <= Configer.BoxConfigList.DoorPossibility ? EDoorType.Wooden : EDoorType.None;
        }
        
        public EWallType WallType = wallType;
        public Observable<bool> Visited = new (false);
        [SerializeField]
        EDoorType doorType = doorType == EDoorType.Random ? RandomDoor() : doorType;
        public bool HasDoor => doorType != EDoorType.None;
        public Observable<bool> Opened = new (false);
    }
    
}