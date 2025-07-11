using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    [InitializeOnLoad]
    public static class TypeCache
    { 
        /// <summary>
        /// ActionNode, CompositeNode, DecoratorNode, GuardNode, RootNode
        /// </summary>
        public static readonly List<Type> NodeGeneralTypes;
        public static readonly Dictionary<string, Type> PortTypeDic;
        /// <summary>
        /// kvp = (ActionNode, [ActionNodeXXX, ActionNodeXXX, ActionNodeXXX, ...])
        /// </summary>
        public static readonly Dictionary<Type, List<Type>> GeneralToSelectionsDic;

        public static readonly HashSet<string> PortPropertyNames;
        static TypeCache()
        {
            NodeGeneralTypes = typeof(NodeBase).Assembly.GetTypes()
                .Where(type =>
                    type.InheritsFrom(typeof(NodeBase))
                    && type != typeof(NodeBase)
                    && !type.IsAbstract
                    && (type.BaseType?.IsAbstract ?? false)
                ).ToList();

            PortTypeDic = new Dictionary<string, Type>();
            typeof(SinglePortData).Assembly
                .GetTypes()
                .Where(x => x.HasAttribute<PortTypeAttribute>())
                .ForEach(x => PortTypeDic.Add(x.Name, x));
            
            GeneralToSelectionsDic = new Dictionary<Type, List<Type>>();
            NodeGeneralTypes.ForEach(generalType =>
            {
                GeneralToSelectionsDic.Add(generalType,
                    generalType.SubType().ToList().Count != 0 ? 
                        generalType.SubType().ToList() :
                        new List<Type>(){generalType});
            });

            PortPropertyNames = typeof(NodeBaseEditor<>)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.PropertyType == typeof(Port))
                .Select(p => p.Name)
                .ToHashSet();
        }
    }
}