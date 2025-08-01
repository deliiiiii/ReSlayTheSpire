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
    public int Energy;
    public int Count = 1;
}