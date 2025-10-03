using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BTNodeEditor : NodeEditor<BTNodeData>
    {
        public BTNodeEditor(BTNodeData nodeData, bool isDefault) 
            : base(nodeData, isDefault){}

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

        public override NodeData CreateNodeData()
        {
            return ScriptableObject.CreateInstance<BTNodeData>();
        }
    }
}