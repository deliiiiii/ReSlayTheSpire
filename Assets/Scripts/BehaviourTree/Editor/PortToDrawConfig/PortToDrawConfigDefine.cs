using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
// ReSharper disable InconsistentNaming

namespace BehaviourTree
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
        [ShowIf(nameof(IsValid))]
        public Direction Direction;
        [ShowIf(nameof(IsValid))]
        public Port.Capacity Capacity;
        [ShowIf(nameof(IsValid))]
        [ValueDropdown(nameof(GetPortType))]
        public string PortTypeName;
        [ShowIf(nameof(IsValid))]
        public string PortName;
        
        static IEnumerable GetPortType()
        {
            return TypeCache.PortTypeDic.Keys;
        }
    }
}