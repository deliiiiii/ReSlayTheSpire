using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace BehaviourTree
{
    public static class TypeCache
    {
        /// <summary>
        /// 缓存每个NodeBaseEditor的DropDownFieldData
        /// kvp = ActionNodeEditor, [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        public static Dictionary<Type, List<Type>> EditorToSubNodeDic;
        // ActionNode, CompositeNode, DecoratorNode, GuardNode, RootNode
        public static List<Type> NodeTypes;

        static TypeCache()
        {
            // MyDebug.Log("TypeCache static constructor called. Initializing EditorToSubNodeDic and NodeTypes.");
            EditorToSubNodeDic = new Dictionary<Type, List<Type>>();
            var baseEditorType = typeof(NodeBaseEditor<>);
            baseEditorType.Assembly.GetTypes()
                .Where(type =>
                    type.InheritsFrom(baseEditorType)
                    && type != baseEditorType
                    && !type.IsAbstract
                    && (type.BaseType?.IsAbstract ?? false)
                )
                // nodeEditorType: ActionNodeEditor or CompositeNodeEditor or DecoratorNodeEditor ...
                .ForEach(nodeEditorType =>
                {
                    EditorToSubNodeDic[nodeEditorType] = new List<Type>();
                    var tBaseType = nodeEditorType.BaseType!.GetGenericArguments()[0];
                    var tSubTypes = tBaseType.SubType().ToList();
                    if (!tSubTypes.Any())
                    {
                        EditorToSubNodeDic[nodeEditorType].Add(tBaseType);
                    }
                    else
                    {
                        tSubTypes.ForEach(tSubType =>
                        {
                            EditorToSubNodeDic[nodeEditorType].Add(tSubType);
                        });
                    }
                });
            
            NodeTypes = typeof(NodeBase).Assembly.GetTypes()
                .Where(type =>
                    type.InheritsFrom(typeof(NodeBase))
                    && type != typeof(NodeBase)
                    && !type.IsAbstract
                    && (type.BaseType?.IsAbstract ?? false)
                ).ToList();
        }
        
        
        public static Type GetEditorByConcreteSubType(Type subType)
        {
            foreach (var kvp in EditorToSubNodeDic)
            {
                if (kvp.Value.Contains(subType))
                {
                    return kvp.Key;
                }
            }
            return null;
        }
        
        public static Type GetTypeByName(string typeName)
        {
            return NodeTypes.First(type => type.Name == typeName);
        }
    }
}