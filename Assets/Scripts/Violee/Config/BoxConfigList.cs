using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(BoxConfigList), menuName = "Violee/" + nameof(BoxConfigList))]
    public class BoxConfigList : ScriptableObject
    {
        [Range(0, 1)]
        public float DoorPossibility;
        [Range(0.4f, 0.5f)]
        public float WalkInTolerance = 0.45f;
        public List<BoxConfig> BoxConfigs = [];
    }
}