// using System;
// using Sirenix.OdinInspector;
//
// namespace BehaviourTree
// {
//     [Serializable]
//     public class ActionNodeDelay : ActionNode, IShowDetail
//     {
//         
//
//         protected ActionNodeDelay()
//         {
//             OnEnter += () => DelaySecondsTimer = 0;
//             OnContinue += dt =>
//             {
//                 DelaySecondsTimer += dt;
//                 IsFinished = DelaySecondsTimer >= DelaySeconds;
//             };
//         }
//
//         public string GetDetail()
//         {
//             string t = DelaySecondsTimer switch
//             {
//                 < 1 => $"{DelaySecondsTimer * 1000:F0}",
//                 < 60 => $"{DelaySecondsTimer:F2}",
//                 _ => $"{DelaySecondsTimer / 60:F2}",
//             };
//             string tTanni = (Timer: DelaySecondsTimer, DelaySeconds) switch
//             {
//                 (<= 1,<= 1) or
//                 (>1 and <=60,>1 and <=60) or
//                 (>60 and <=3600,>60 and <=3600) => "",
//                 (<=1, _) => "ms",
//                 (<=60, _) => "s",
//                 (_, _) => "min",
//             };
//             string dWithTanni = DelaySeconds switch
//             {
//                 < 1 => $"{DelaySeconds * 1000:F0}ms",
//                 < 60 => $"{DelaySeconds:F2}s",
//                 _ => $"{DelaySeconds / 60:F2}min",
//             };
//             return $"{t}{tTanni}/{dWithTanni}";
//         }
//     }
// }