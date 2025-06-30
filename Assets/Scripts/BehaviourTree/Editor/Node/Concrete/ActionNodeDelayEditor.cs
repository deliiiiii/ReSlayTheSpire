using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    
    public class ActionNodeDelayEditor : ActionNodeEditor<ActionNodeDelay>
    {
        public ActionNodeDelayEditor()
        {
            title = "Action Node Delay";
            viewDataKey = "ActionNodeDelay_001";
            NodeBase = new ActionNodeDelay(0f);
            
            var delaySecondsField = new FloatField("Delay Seconds")
            {
                value = NodeBase.DelaySeconds
            };
            delaySecondsField.RegisterValueChangedCallback(v =>
            {
                NodeBase.DelaySeconds = v.newValue;
            });
            extensionContainer.Add(delaySecondsField);
        }
    }
}