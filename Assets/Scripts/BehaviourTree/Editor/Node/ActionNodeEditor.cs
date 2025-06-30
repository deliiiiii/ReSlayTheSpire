using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public abstract class ActionNodeEditor<T> : NodeBaseEditor<T> where T : ActionNode
    {
        static List<Type> typeList;
        static Dictionary<string, Type> dic;
        
        public ActionNodeEditor()
        {
            viewDataKey = "ActionNode_001";
            if (typeList == null)
            {
                var types = typeof(ActionNode).Assembly.GetTypes();
                typeList = new List<Type>();
                dic = new Dictionary<string, Type>();
                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(ActionNode)))
                        continue;
                    typeList.Add(type);
                    dic[type.Name] = type;
                }
            }
            
            if (typeList.Count == 0)
            {
                MyDebug.LogError($"No {nameof(ActionNode)} types found in the assembly.");
                return;
            }
            var typeField = new DropdownField(dic.Keys.ToList(), typeList[0].Name);
            title = typeList[0].Name;
            NodeBase = Activator.CreateInstance(typeList[0]) as T;
                
            typeField.RegisterValueChangedCallback(choice =>
            {
                title = choice.newValue;
                NodeBase = Activator.CreateInstance(dic[choice.newValue]) as T;
            });
            extensionContainer.Add(typeField);

            var outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Parent ↑";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}