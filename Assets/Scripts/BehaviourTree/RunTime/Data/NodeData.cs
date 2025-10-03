using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

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
public abstract class BTNodeData : NodeData
{
    #region Consistent
    public readonly Observable<EState> State = new(EState.Failed);
    #endregion
    
    #region Runtime
    [CanBeNull]
    public GuardNode GuardNode { get;private set; }
    
    // TODO OfType性能
    protected LinkedList<BTNodeData> ChildLinkedList => new(ChildList.OfType<BTNodeData>());
    public BTNodeData LastChild => ChildCount == 0 ? null : ChildList[^1] as BTNodeData;

    public override void OnDeserializeEnd()
    {
        // TODO OfType性能
        base.OnDeserializeEnd();
        GuardNode = ChildList.OfType<GuardNode>().FirstOrDefault();
    }

    protected bool CheckGuard()
    {
        return GuardNode?.Judge() ?? true;
    }
    #endregion
    
    
    #region Tick
    public EState TickTemplate(float dt)
    {
        if (!CheckGuard())
        {
            RecursiveDo(Reset);
            return State.Value = EState.Failed;
        }
        return State.Value = Tick(dt);
    }
    /// <summary>
    /// 默认Tick最后一个节点， 没有节点时返回Succeeded
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    protected virtual EState Tick(float dt)
    {
        return LastChild?.TickTemplate(dt) ?? EState.Succeeded;
    }
    
    protected virtual void Reset()
    {
        State.Value = EState.Failed;
    }
    #endregion
    
    public virtual string GetDetail() => string.Empty;
}

[Serializable]
public abstract class NodeData : SerializedScriptableObject
{
    #region Consistent
    [HideInInspector] public Vector2 Position;
    [HideInInspector] public Vector2 Size;
    // public string Description;
    [ReadOnly] public int NodeID;
    protected abstract EChildCountType childCountType { get; set; }
    public List<NodeData> ChildList = new();
    #endregion
    
    public virtual void OnDeserializeEnd(){}
    
    #region Runtime
    // [HideInInspector]
    public int ChildCount => ChildList?.Count ?? 0;
    

    public void AddChild(NodeData child)
    {
        switch (childCountType)
        {
            case EChildCountType.Single when ChildCount == 0:
            case EChildCountType.Multiple:
                ChildList ??= new List<NodeData>();
                ChildList.Add(child);
                break;
            case EChildCountType.None:
            default:
                MyDebug.LogError($"Node {GetType().Name} can not have child nodes");
                break;
        }
    }
    protected bool RemoveChild(NodeData child)
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

    public virtual void OnRefreshTreeEnd(){}

    public void RecursiveDo(Action func)
    {
        func();
        ChildList?.ForEach(c => c.RecursiveDo(func));
    }
    
    public Type GetGeneralType()
    {
        var ret = GetType();
        while (ret.BaseType is { IsAbstract: false })
        {
            ret = ret.BaseType;
        }

        return ret;
    }
}

}