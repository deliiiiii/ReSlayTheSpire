using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviourTree
{
    
[Serializable]
public abstract class ACDNode : NodeBase
{
    #region Tick

    public GuardNode GuardNode;
    public abstract EState OnTick(float dt);
    public abstract void OnFail();

    void OnEnable()
    {
        GuardNode = CreateInstance<GuardNodeAlwaysTrue>();
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
    
    /// <summary>
    /// 仅检查当前节点的Guard条件
    /// </summary>
    /// <returns>Guard失败返回当前节点，否则返回null</returns>
    bool CheckGuardLocal()
    {
        return GuardNode.Condition();
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
    
    #endregion

    #region Parent & Child & Guard
    [JsonIgnore]
    public ACDNode Parent { get; set; }
    protected ACDNode FirstChild => ChildList?.Last?.Value;
    [ShowInInspector][JsonProperty]
    protected LinkedList<ACDNode> ChildList { get; set; }
#if UNITY_EDITOR
    public abstract ACDNode AddChild(ACDNode child);
    public bool RemoveChild(ACDNode child)
    {
        if (ChildList != null && ChildList.Contains(child))
        {
            ChildList.Remove(child);
            return true;
        }
        return false;
    }
    public ACDNode ClearChildren()
    {
        ChildList?.Clear();
        return this;
    }
    public void SetGuard(GuardNode guardNode)
    {
        GuardNode = guardNode;
    }
    
    
    public override void OnSave()
    {
        AssetDatabase.AddObjectToAsset(GuardNode, this);
        ChildList?.ForEach(child =>
        {
            AssetDatabase.AddObjectToAsset(child, this);
            child.OnSave();
        });
    }
#endif
    #endregion
}
}