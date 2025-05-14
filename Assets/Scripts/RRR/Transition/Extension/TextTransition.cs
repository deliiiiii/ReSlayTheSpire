// using System;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace SubstanceP
// {
//     [TransitionTarget(typeof(Text))]
//     public partial class TextTransition : Transition
//     {
//         [TransitionField("text")] private string txt;
//
//         private partial void InitTxt(Text target) => target.text = txt;
//         //
//         private partial void ProcessTxt(Text target, string delta) => target.text = Plus(target.text, delta);
//
//         public static string Plus(string s1, string s2)
//         {
//             if (!int.TryParse(s1, out int i1) || !int.TryParse(s2, out int i2))
//             {
//                 return "NAN????";
//             }
//             return (i1 + i2).ToString();
//         }
//         public static string Minus(string s1, string s2)
//         {
//             if (!int.TryParse(s1, out int i1) || !int.TryParse(s2, out int i2))
//             {
//                 return "NAN????";
//             }
//             return (i1 - i2).ToString();
//         }
//
//     }
//
// }