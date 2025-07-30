using System;
using UnityEngine;

namespace Violee;

[Serializable]
public class MiniItemConfig
{
    public int ID;
    public required Sprite Sprite;
    public required string Description;
    public int InitValue;
}