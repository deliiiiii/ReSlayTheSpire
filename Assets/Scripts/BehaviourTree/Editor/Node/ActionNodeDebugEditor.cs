using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    
    public class ActionNodeDebugEditor : NodeBaseEditor<ActionNodeDebug>
    {
        public ActionNodeDebugEditor()
        {
            title = "Action Node Debug";
            viewDataKey = "ActionNodeDebug_001";
            NodeBase = new ActionNodeDebug("Debug Message");
            
            var debugTypeField = new EnumField(nameof(NodeBase.DebugType), EDebugType.Log);
            debugTypeField.RegisterValueChangedCallback(v =>
                {
                    NodeBase.DebugType = (EDebugType)v.newValue;
                });
            extensionContainer.Add(debugTypeField);
            
            var contentField = new TextField("Content")
            {
                value = NodeBase.Content = "Debug SomeThing",
                multiline = true
            };
            contentField.RegisterValueChangedCallback(v =>
            {
                NodeBase.Content = v.newValue;
            });
            extensionContainer.Add(contentField);
            
        }
    }
}