using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    [Serializable]
    public class BTNodeEditor<T> : NodeEditorBase where T : BTNodeData
    {
        public BTNodeEditor(NodeData nodeData, bool isDefault) : base(nodeData, isDefault)
        {
        }

        public new T NodeData
        {
            get => base.NodeData as T;
            set => base.NodeData = value;
        }

        protected override void DrawNodeField()
        {
            base.DrawNodeField();
            
            style.backgroundColor = tickStateColorDic[NodeData.State];
            NodeData.State.OnValueChangedAfter += evt =>
            {
                style.backgroundColor = tickStateColorDic[evt];
            };
            
            var detailLabel = new Label(NodeData.GetDetail())
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    color = Color.white,
                    fontSize = 15,
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            };
            detailLabel.schedule.Execute(() =>
            {
                detailLabel.text = NodeData.GetDetail();
            }).Every(10);
            extensionContainer.Add(detailLabel);
        }
    }
}