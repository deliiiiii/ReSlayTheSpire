using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

public enum EChildCountType
{
    None,
    Single,
    Multiple,
}

[Serializable]
public abstract class NodeBase : ScriptableObject
{
    [HideInInspector]
    public Rect RectInGraph;
    [HideInInspector]
    public Observable<EState> State = new(EState.Failed);

    [HideInInspector] [CanBeNull] public GuardNode GuardNode;
    protected abstract EChildCountType childCountType { get; set; }
    
    public NodeBase LastChild => ChildLinkedList?.Last?.Value;
    protected LinkedList<NodeBase> ChildLinkedList => ChildList == null ? new() : new LinkedList<NodeBase>(ChildList);
    [HideInInspector]
    public List<NodeBase> ChildList;

    public void AddChild(NodeBase child)
    {
        switch (childCountType)
        {
            case EChildCountType.Single when LastChild == null:
            case EChildCountType.Multiple:
                ChildList ??= new List<NodeBase>();
                ChildList.Add(child);
                break;
            case EChildCountType.None:
            default:
                break;
        }
    }
    protected bool RemoveChild(NodeBase child)
    {
        if (ChildList != null && ChildList.Contains(child))
        {
            ChildList.Remove(child);
            return true;
        }
        return false;
    }
    public void ClearChildren()
    {
        ChildList?.Clear();
    }

    public virtual EState Tick(float dt)
    {
        return EState.Succeeded;
    }

    protected virtual EState OnTickChild(float dt)
    {
        return EState.Succeeded;
    }

    protected static void OnFail(NodeBase target)
    {
        if(!target)
            return;
        target.State.Value = EState.Failed;
    }
    
    protected static void OnResetState(NodeBase target)
    {
        if(!target)
            return;
        target.State.Value = EState.Failed;
    }
    
    protected void RecursiveDo(Action<NodeBase> func)
    {
        func(this);
        func(GuardNode);
        ChildList?.ForEach(func);
    }
}

}