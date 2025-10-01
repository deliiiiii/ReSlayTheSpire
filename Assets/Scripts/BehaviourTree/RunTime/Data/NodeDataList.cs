using System;
using System.Collections.ObjectModel;

namespace BehaviourTree
{
    [Serializable]
    public class NodeDataList : KeyedCollection<int, NodeData>
    {
        protected override int GetKeyForItem(NodeData item)
        {
            return item.NodeID;
        }
    }
}