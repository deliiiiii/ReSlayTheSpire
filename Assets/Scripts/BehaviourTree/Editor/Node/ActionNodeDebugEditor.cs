using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public enum EDebugType
    {
        Log,
        Warning,
        Error
    }
    public class ActionNodeDebugEditor : NodeBaseEditor<ActionNodeDebug>
    {
        public EDebugType DebugType = EDebugType.Log;
        public ActionNodeDebugEditor()
        {
            title = "Action Node Debug";
            viewDataKey = "ActionNodeDebug_001";
            NodeBase = new ActionNodeDebug("Debug Message");
            var enumField = new EnumField(nameof(DebugType), EDebugType.Log);
            enumField.RegisterValueChangedCallback(v =>
                {
                    DebugType = (EDebugType)v.newValue;
                });
            extensionContainer.Add(enumField);
        }
    }
}