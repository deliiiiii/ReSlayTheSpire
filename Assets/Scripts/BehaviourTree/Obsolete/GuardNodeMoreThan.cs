// using System;
//
// namespace BehaviourTree
// {
//     [Serializable]
//     public class GuardNodeMoreThan : GuardNode, IShowDetail
//     {
//         public int Value;
//         public int Threshold;
//
//         void OnEnable()
//         {
//             Condition = () => Value > Threshold;
//         }
//
//         public string GetDetail()
//         {
//             return $"{Value} > {Threshold}";
//         }
//     }
// }