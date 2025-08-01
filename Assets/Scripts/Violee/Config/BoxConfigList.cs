using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(BoxConfigList), menuName = "Violee/" + nameof(BoxConfigList))]
    public class BoxConfigList : ScriptableObject
    {
        public float DoorPossibility;
        [MinValue(0.4)][MaxValue(0.5)]
        public float WalkInTolerance = 0.45f;
        public List<BoxConfig> BoxConfigs = [];
    }
}