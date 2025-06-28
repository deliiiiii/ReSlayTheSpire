using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace BehaviourTree
{

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
    
    public bool Tick(float dt)
    {
        var failedNode = CheckGuard();
        if(failedNode != null)
        {
            var curNode = this;
            do
            {
                MyDebug.Log($"\"{curNode.Name}\" Guard failed", LogType.Tick);
                curNode = curNode.Parent;
            } while (curNode != failedNode);
        }
        var tickResult = OnTick(dt);
        MyDebug.Log($"\"{Name}\" Tick: {tickResult}", LogType.Tick);
        return tickResult;
    }

    NodeBase CheckGuard()
    {
        return !Guard.Condition() ? this : Parent?.CheckGuard();
        // if (!Guard.Condition())
        //     return this;
        // if (Parent == null)
        //     return null;
        // return Parent.CheckGuard();
    }
    
    
    
    
    
}


}