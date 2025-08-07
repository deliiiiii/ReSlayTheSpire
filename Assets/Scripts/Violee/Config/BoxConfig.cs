using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

[Serializable]
public class BoxConfig
{
    [HideInInspector] public string Name = string.Empty;
    [HideInInspector] public byte Walls;
    [ReadOnly] public required Texture2D Texture2D;
    public int BasicWeight;
}