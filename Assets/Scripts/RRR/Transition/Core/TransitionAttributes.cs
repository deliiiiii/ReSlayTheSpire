using System;

namespace SubstanceP
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TransitionTargetAttribute : Attribute
    {
        public Type Target { get; private set; }
        public TransitionTargetAttribute(Type target) => Target = target;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TransitionFieldAttribute : Attribute
    {
        public string Relative { get; private set; }
        public TransitionFieldAttribute(string relative) => Relative = relative;
    }
}
