using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviourTree
{


public enum EState
{
    Succeeded,
    Failed,
    Running,
}


[Serializable]
public abstract class NodeBase : ScriptableObject
{
    public string NodeName;
#if UNITY_EDITOR
    public abstract void OnSave();
#endif
}

}