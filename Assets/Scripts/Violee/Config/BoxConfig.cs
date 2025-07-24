using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(BoxConfig), menuName = "Violee/" + nameof(BoxConfig))]
    public class BoxConfig : ScriptableObject
    {
        public float DoorPossibility;
        public List<BoxConfigSingle> BoxConfigList = [];
        [MinValue(0.4)][MaxValue(0.5)]
        public float WalkInTolerance = 0.45f;
    }
}