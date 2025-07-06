using System;
using Sirenix.OdinInspector;

namespace BehaviourTree
{
    // [Serializable]
    // public class ActionNodeSet<T> : ActionNode
    // {
    //     [DrawnField]
    //     public T TarValue;
    //     public ActionNodeSet(){}
    //     public ActionNodeSet(T tarValue, Action<T> setter)
    //     {
    //         this.TarValue = tarValue;
    //         OnContinue = _ =>
    //         {
    //             setter(this.TarValue);
    //             isFinished = true;
    //         };
    //     }
    //
    //     public override string ToString()
    //     {
    //         return $"Set to {TarValue}";
    //     }
    // }
}