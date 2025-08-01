using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;


[Serializable]
public abstract class SceneItemConfig
{
    public required string Name;
    public required GameObject Object;

    #region HasCount
    public bool HasCount;
    [Header("HasCount")]
    [ShowIf(nameof(HasCount))] public int Count;
    #endregion
}

[Serializable]
public class PurpleSceneItemConfig : SceneItemConfig
{
    [Header("Purple")]
    public int Energy;
}