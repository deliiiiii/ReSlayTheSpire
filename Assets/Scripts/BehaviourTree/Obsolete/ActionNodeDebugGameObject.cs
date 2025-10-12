// using System;
// using UnityEngine;
//
// namespace BehaviourTree
// {
//     public class ActionNodeDebugGameObject : ActionNode
//     {
//         public GameObject Go;
//
//         protected override void OnEnableAfter()
//         {
//             OnContinue = _ =>
//             {
//                 MyDebug.Log($" pos {Go.transform.localPosition}");
//                 IsFinished = true;
//             };
//         }
//     }
// }