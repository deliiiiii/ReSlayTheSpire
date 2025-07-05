using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{


public enum EState
{
    Succeeded,
    Failed,
    Running,
}

public interface IHasChild
{
    public IEnumerable<ACDNode> ChildNodes { get; }
}

[Serializable]
public abstract class NodeBase : ScriptableObject
{
    public string NodeName;
    public Rect RectInGraph;
}

}