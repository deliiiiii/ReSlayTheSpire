using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public abstract class DecorateNodeEditor<T> : NodeBaseEditor<T> where T : DecorateNode
    {
        static List<Type> decorateNodeTypeList;
        static Dictionary<string, Type> dic;
        
        public DecorateNodeEditor()
        {
            // title = "DecorateNode";
            viewDataKey = "DecorateNode_001";
            
            if (decorateNodeTypeList == null)
            {
                var types =  typeof(DecorateNode).Assembly.GetTypes();
                decorateNodeTypeList = new List<Type>();
                dic = new Dictionary<string, Type>();
                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(DecorateNode)))
                        continue;
                    decorateNodeTypeList.Add(type);
                    dic[type.Name] = type;
                }
            }
            
            if (decorateNodeTypeList.Count == 0)
            {
                MyDebug.LogError("No DecorateNode types found in the assembly.");
                return;
            }
            var decorateTypeField = new DropdownField(dic.Keys.ToList(), decorateNodeTypeList[0].Name);
            title = decorateNodeTypeList[0].Name;
            NodeBase = Activator.CreateInstance(decorateNodeTypeList[0]) as T;
                
            decorateTypeField.RegisterValueChangedCallback(choice =>
            {
                title = choice.newValue;
                NodeBase = Activator.CreateInstance(dic[choice.newValue]) as T;
            });
            extensionContainer.Add(decorateTypeField);
            
            var inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            inputPort.portName = "Dec ↓";
            inputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBaseEditor<NodeBase>));
            outputPort.portName = "Parent ↑";
            outputPort.tooltip = typeof(NodeBaseEditor<NodeBase>).ToString();
            outputContainer.Add(outputPort);
        }
    }
}