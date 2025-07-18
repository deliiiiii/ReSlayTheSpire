using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(BoxConfig), menuName = "Violee/" + nameof(BoxConfig))]
    public class BoxConfig : ScriptableObject
    {
        public float DoorPossibility;
        public List<BoxConfigSingle> BoxConfigList;
    }
}