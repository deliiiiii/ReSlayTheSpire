using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    [Serializable]
    public abstract class NodeEditor<T> 
        : NodeEditorBase
        where T : NodeData
    {
        protected NodeEditor(NodeData nodeData, bool isDefault) : base(nodeData, isDefault){}
        public new T NodeData
        {
            get => base.NodeData as T;
            set => base.NodeData = value;
        }
    }
}