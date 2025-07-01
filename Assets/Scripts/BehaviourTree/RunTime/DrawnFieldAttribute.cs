using System;

namespace BehaviourTree
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DrawnFieldAttribute : Attribute
    {
        
    }
}