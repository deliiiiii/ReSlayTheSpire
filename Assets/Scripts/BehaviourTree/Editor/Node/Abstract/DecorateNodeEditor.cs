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
    }
}