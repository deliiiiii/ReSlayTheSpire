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
    public Vector2 Position;
    [HideInInspector]
    public Vector2 Size;
    public readonly Observable<EState> State = new(EState.Failed);

    protected virtual void OnEnable()
    {
        Binder.From(State).To(s =>
        {
            if (s == EState.Failed)
                OnFail();
        });
    }
    
    
    #region Guard
    [HideInInspector][CanBeNull] public GuardNode GuardNode;
    bool CheckGuard()
    {
        return GuardNode?.Judge() ?? true;
    }
    #endregion
    
    
    #region Child
    [HideInInspector]
    public List<NodeBase> ChildList;
    public NodeBase LastChild => ChildLinkedList?.Last?.Value;
    protected abstract EChildCountType childCountType { get; set; }
    protected LinkedList<NodeBase> ChildLinkedList => ChildList == null ? new() : new LinkedList<NodeBase>(ChildList);

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
    #endregion

    
    #region Tick
    public EState Tick(float dt)
    {
        if (!CheckGuard())
        {
            RecursiveDo(MyReset);
            return State.Value = EState.Failed;
        }
        return State.Value = OnTickChild(dt);
    }
    /// <summary>
    /// 默认Tick最后一个节点， 没有节点时返回Succeeded
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    protected virtual EState OnTickChild(float dt)
    {
        return LastChild?.Tick(dt) ?? EState.Succeeded;
    }
    protected static void MyReset(NodeBase target)
    {
        if(!target)
            return;
        target.State.Value = EState.Failed;
    }

    protected virtual void OnFail(){}
    #endregion
    
    protected void RecursiveDo(Action<NodeBase> func)
    {
        func(this);
        func(GuardNode);
        ChildList?.ForEach(c => c.RecursiveDo(func));
    }

    public Type GetGeneralType()
    {
        var ret = GetType();
        while (ret.BaseType != null && !ret.BaseType.IsAbstract)
        {
            ret = ret.BaseType;
        }

        return ret;
    }
}

}