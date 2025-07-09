using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    
    #region Guard
    [HideInInspector][CanBeNull] public GuardNode GuardNode;
    bool CheckGuard()
    {
        return !GuardNode || GuardNode.Judge();
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
    public async Task<EState> TickAsync(float dt)
    {
        if (!CheckGuard())
        {
            RecursiveDo(OnFail);
            return State.Value = EState.Failed;
        }
        State.Value = await OnTickChild(dt);
        // MyDebug.Log($"\"{NodeName}\" Tick: {tickResult}", LogType.Tick);
        return State.Value;
    }
    /// <summary>
    /// 默认Tick最后一个节点， 没有节点时返回Succeeded
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    protected virtual async Task<EState> OnTickChild(float dt)
    {
        if(LastChild == null)
            return EState.Succeeded;
        return await LastChild.TickAsync(dt);
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
    #endregion

    
    #region Interface
    public static void SetBlackboard(NodeBase target, Blackboard blackboard)
    {
        if (target is IRequireBlackBoard req)
        {
            req.Blackboard = blackboard;
        }
    }
    #endregion Interface
    
    
    
    protected void RecursiveDo(Action<NodeBase> func)
    {
        func(this);
        if(GuardNode != null)
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