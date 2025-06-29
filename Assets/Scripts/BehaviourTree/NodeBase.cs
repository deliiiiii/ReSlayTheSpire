using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace BehaviourTree
{
    
public static class NodeBaseExtensions
{
    public static T SetName<T>(this T node, string name) where T : NodeBase
    {
        node.Name = name;
        return node;
    }

    public static T SetGuard<T>(this T node, Func<bool> condition) where T : NodeBase
    {
        node.Guard = new Guard { Condition = condition };
        return node;
    }
    public static T RemoveGuard<T>(this T node) where T : NodeBase
    {
        node.Guard = Guard.AlwaysTrue;
        return node;
    }
}


[Serializable]
public abstract class NodeBase
{
    public string Name = "New Node";
    [HideInInspector]
    public Tree Tree;
    
    public Guard Guard = Guard.AlwaysTrue;
    [HideInInspector]
    public NodeBase Parent;



    public abstract bool OnTick(float dt);
    public abstract void OnFail();
    public abstract NodeBase GetChild();
    
    // public T SetName<T>(string name) where T : NodeBase
    // {
    //     Name = name;
    //     return this as T;
    // }

    public NodeBase Back()
    {
        return Parent;
    }
    
    public bool Tick(float dt)
    {
        if (!CheckGuardLocal())
        {
            OnFail();
            return false;
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
        MyDebug.Log($"\"{Name}\" Tick: {tickResult}", LogType.Tick);
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