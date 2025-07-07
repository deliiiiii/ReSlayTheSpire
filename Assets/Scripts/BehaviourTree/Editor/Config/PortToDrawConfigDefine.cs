using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Config
{
    [Serializable]
    public enum EPortType
    {
        Guard,
        HasChild,
    }
    
    [Serializable]
    public class SinglePortData
    {
        public bool IsValid;
        public Direction Direction;
        public Port.Capacity Capacity;
        public EPortType PortType;
        public string Name;
    }
}