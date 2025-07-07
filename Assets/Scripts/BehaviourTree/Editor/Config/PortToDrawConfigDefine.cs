using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
// ReSharper disable InconsistentNaming

namespace BehaviourTree.Config
{
    [PortType]
    interface GuardPort{}
    [PortType]
    interface HasChildPort{}
    
    [PortType]
    interface AllPort : GuardPort, HasChildPort{}
    
    [Serializable]
    public class SinglePortData
    {
        public bool IsValid;
        public Direction Direction;
        public Port.Capacity Capacity;
        [ValueDropdown(nameof(GetPortType))]
        public string PortTypeName;
        public string PortName;
        
        static IEnumerable GetPortType()
        {
            return TypeCache.PortTypeDic.Keys;
        }
    }
}