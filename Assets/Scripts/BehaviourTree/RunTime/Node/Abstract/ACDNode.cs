using System;

namespace BehaviourTree
{
    
[Serializable]
public abstract class ACDNode : NodeBase
{
    #region Tick
    public override EState Tick(float dt)
    {
        if (!CheckGuardLocal())
        {
            RecursiveDo(OnFail);
            return State.Value = EState.Failed;
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
        State.Value = OnTickChild(dt);
        // MyDebug.Log($"\"{NodeName}\" Tick: {tickResult}", LogType.Tick);
        return State.Value;
    }
    
    /// <summary>
    /// 仅检查当前节点的Guard条件
    /// </summary>
    /// <returns>Guard失败返回当前节点，否则返回null</returns>
    bool CheckGuardLocal()
    {
        return !GuardNode || GuardNode.Judge();
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
}
}