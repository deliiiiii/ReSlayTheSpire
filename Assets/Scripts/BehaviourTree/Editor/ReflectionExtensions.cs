using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    public static class ReflectionExtensions
    {
        // public static Type FirstSubType(this Type parentType)
        // {
        //     return parentType.SubType().FirstOrDefault();
        // }
        
        public static IEnumerable<Type> SubType(this Type parentType)
        {
            return parentType.Assembly.GetTypes().Where(x => x.IsSubclassOf(parentType));
        }
        
    }
}