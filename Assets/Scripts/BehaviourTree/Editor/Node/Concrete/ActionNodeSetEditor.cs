using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class ActionNodeSetEditor<TValue> : ActionNodeEditor<ActionNodeSet<TValue>> //where TValue : struct
    {
        public ActionNodeSetEditor()
        {
            //为TValue类型创建一个输入框
            var type = typeof(TValue);
            BindableElement inputField = null;
            if (type == typeof(int))
            {
                var added = new IntegerField(0);
                added.RegisterValueChangedCallback(v =>
                {
                    NodeBase.TarValue = (dynamic)v.newValue;
                });
                inputField = added;
            }
            
            extensionContainer.Add(inputField);
        }

        
    }
}