using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;


[Serializable]
public abstract class SceneItemConfig : ScriptableObject
{
    public required GameObject Object;

    #region HasCount
    public bool HasCount;
    [Header("HasCount")]
    [ShowIf(nameof(HasCount))] public int Count;
    #endregion
}