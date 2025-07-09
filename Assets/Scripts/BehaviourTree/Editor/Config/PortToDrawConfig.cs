using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

using UnityEditor.Experimental.GraphView;

namespace BehaviourTree.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(PortToDrawConfig), menuName = "BehaviourTree/" + nameof(PortToDrawConfig))]
    public class PortToDrawConfig : ScriptableObject
    {
        public SerializableDictionary<string, SerializableDictionary<string, SinglePortData>> TypeToPortToDrawData;
        void OnEnable()
        {
            EditorUtility.SetDirty(this);
            
            
            // // 删除不存在的节点类型
            var nodeTypeNames = TypeCache.NodeGeneralTypes.Select(x => x.Name);
            TypeToPortToDrawData.RemoveAll(kvp => !nodeTypeNames.Contains(kvp.Key));
            
            foreach (var nodeType in TypeCache.NodeGeneralTypes)
            {
                TypeToPortToDrawData.TryAdd(nodeType.Name, new SerializableDictionary<string, SinglePortData>());
                var portToDrawData = TypeToPortToDrawData[nodeType.Name];
                // 删除不存在的端口字段
                portToDrawData.RemoveAll(kvp => !TypeCache.PortPropertyNames.Contains(kvp.Key));
                foreach (var portPropertyName in TypeCache.PortPropertyNames)
                {
                    portToDrawData.TryAdd(portPropertyName, new SinglePortData());
                }
            }

        }
    }
}
