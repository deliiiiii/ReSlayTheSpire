// using System;
// using System.Text;
// using UniRx;
// using UnityEngine;
//
// public class Test : MonoBehaviour
// {
//     [SerializeField]
//     float f = 250f;
//     void Awake()
//     {
//         //     var h = Tween
//         //         .Linear(3f)
//         //         .Process(x =>
//         //         {
//         //             MyDebug.Log($" T {x}");
//         //         })
//         //         .Finish(() => MyDebug.Log("Teweeen end"));
//         //                         
//         //     Flow.Create()
//         //         .Then(() => MyDebug.Log("F1"))
//         //         .Delay(2f)
//         //         .Then(h)
//         //         .Then(() => MyDebug.Log("Flow End"))
//         //         .Run();
//         // }
//         var stream = Observable.EveryUpdate()
//             .Where(_ => Input.GetKeyDown(KeyCode.D));
//             
//         stream.Buffer(stream.Throttle(TimeSpan.FromMilliseconds(f)))
//             .Where(xs => xs.Count >= 0)
//             .Subscribe(x =>
//             {
//                 var sb = new StringBuilder();
//                 sb.Append($"CCCCount {x.Count}");
//                 foreach (var item in x)
//                 {
//                     sb.Append($" {item} ");
//                 }
//                 MyDebug.Log(sb.ToString());
//             })
//             .AddTo(this);
//     }
//
// }