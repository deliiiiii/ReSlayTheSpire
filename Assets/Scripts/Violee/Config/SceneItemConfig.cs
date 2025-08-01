using System;
using UnityEngine;

namespace Violee;


[Serializable]
public abstract class SceneItemConfig
{
    public required string Name;
    public required GameObject Object;
}

[Serializable]
public class PurpleSceneItemConfig : SceneItemConfig
{
    public readonly int Energy;
    public readonly int Count = 1;
}