using System;
using UnityEngine;

namespace BehaviourTree
{


public enum EState
{
    Succeeded,
    Failed,
    Running,
}

[Serializable]
public abstract class NodeBase
{
    public string NodeName;
}

}