using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
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
    [HideInInspector]
    public Tree Tree;
    public string NodeName = "New Node";
    public Guard Guard = Guard.AlwaysTrue;
    [HideInInspector]
    public NodeBase Parent;
    [ShowInInspector]
    protected LinkedList<NodeBase> childList;
    public NodeBase Back() => Parent;
    public NodeBase ToChild() => childList?.Last?.Value;
    public abstract EState OnTick(float dt);
    public abstract void OnFail();
    public abstract NodeBase AddChild(NodeBase child);
    
    public bool RemoveChild(NodeBase child)
    {
        if (childList != null && childList.Contains(child))
        {
            childList.Remove(child);
            return true;
        }
        MyDebug.LogError($"Node \"{NodeName}\" does not have child \"{child.NodeName}\".");
        return false;
    }
    
    public NodeBase ClearChildren()
    {
        childList?.Clear();
        return this;
    }
    
    public EState Tick(float dt)
    {
        if (!CheckGuardLocal())
        {
            OnFail();
            return EState.Failed;
        }
        // var failedNode = CheckGuardGlobal();
        // if(failedNode != null)
        // {
        //     var curNode = this;
        //     do
        //     {
        //         MyDebug.Log($"\"{curNode.Name}\" Guard failed", LogType.Tick);
        //         curNode.OnFail();
        //         curNode = curNode.Parent;
        //     } while (curNode != failedNode);
        // }
        var tickResult = OnTick(dt);
        MyDebug.Log($"\"{NodeName}\" Tick: {tickResult}", LogType.Tick);
        return tickResult;
    }

    // /// <summary>
    // /// 检查根节点到当前节点的所有Guard条件
    // /// </summary>
    // /// <returns>Guard失败的节点，没有则返回null</returns>
    // NodeBase CheckGuardGlobal()
    // {
    //     return !Guard.Condition() ? this : Parent?.CheckGuardGlobal();
    //     // if (!Guard.Condition())
    //     //     return this;
    //     // if (Parent == null)
    //     //     return null;
    //     // return Parent.CheckGuard();
    // }
    
    /// <summary>
    /// 仅检查当前节点的Guard条件
    /// </summary>
    /// <returns>Guard失败返回当前节点，否则返回null</returns>
    bool CheckGuardLocal()
    {
        
        return Guard.Condition();
    }
    
    
    
    
    
}


}