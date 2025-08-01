// using System;
//
// namespace Violee
// {
//     public class GuardedFunc<TResult>(Func<TResult> func)
//     {
//         public event Func<bool>? Guard;
//         public TResult? TryInvoke()
//         {
//             return !CheckGuard() ? default : func.Invoke();
//         }
//
//         bool CheckGuard()
//         {
//             var ret = Guard?.Invoke() ?? true;
//             if(!ret)
//                 MyDebug.Log($"GuardedFunc<{typeof(TResult).Name}> failed");
//             return ret;
//         }
//     }
// }