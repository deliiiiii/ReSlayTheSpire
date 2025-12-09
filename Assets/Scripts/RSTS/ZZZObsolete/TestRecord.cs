//
// using System;
//
// public record TestRecord
// {
//     public int IntTest;
// }
//
// public class MyClass
// {
//     public TestRecord GetRecord()
//     {
//         var ret = new TestRecord { IntTest = 42 };
//         ret.IntTest = 100;
//         return ret;
//     }
// }
//
// // public class TestSwitch
// // {
// //     public void Test()
// //     {
// //         string? type;
// //         switch (typeof(int), typeof(string))
// //         {
// //             case (typeof(int), typeof(string)):
// //                 type = "int-string";
// //                 break;
// //             default:
// //                 throw new ArgumentOutOfRangeException("(typeof(int), typeof(string))");
// //         }
// //     }
// // }