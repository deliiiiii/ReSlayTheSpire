using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class BoxConfigSingle
    {
        [ReadOnly]
        public string Name;
        [HideInInspector]
        public byte Walls;
        [ReadOnly]
        public Texture2D Texture2D;
        public int BasicWeight;
    }

    
}