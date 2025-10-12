// using System;
//
// namespace BehaviourTree
// {
//     [Serializable]
//     public class NothingNode : DecorateNode
//     {
//         protected override EState OnTickChild(float dt)
//         {
//             var ret = LastChild?.Tick(dt);
//             return ret ?? EState.Succeeded;
//         }
//     }
// }