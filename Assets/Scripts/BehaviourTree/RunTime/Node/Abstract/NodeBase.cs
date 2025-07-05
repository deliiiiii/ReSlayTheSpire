using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{


public enum EState
{
    Failed,
    Succeeded,
    Running,
}

[Serializable]
public abstract class NodeBase : ScriptableObject
{
    public string NodeName;
    public Rect RectInGraph;
    public Observable<EState> State = new(EState.Failed);
}

}