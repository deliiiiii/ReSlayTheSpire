// using System;
//
// public class OuterClass
// {
//     const int OuterInt = 42;
//     public int OuterInt2 = 422;
//     static int outerStaticField = 100;
//     
//     public class InnerClass
//     {
//         int innerInt;   
//         public void AccessOuterMembers(OuterClass outerInstance)
//         {
//             Console.WriteLine(OuterInt);
//             Console.WriteLine(outerInstance.OuterInt2);
//             
//             Console.WriteLine(outerStaticField);
//             
//             outerInstance.OuterMethod();
//         }
//     }
//
//     void OuterMethod()
//     {
//         Console.WriteLine("外部私有方法");
//     }
// }
//
//
//
// public class MainClass
// {
//     public static void Main(string[] args)
//     {
//         OuterClass outer = new OuterClass();
//         OuterClass.InnerClass inner = new OuterClass.InnerClass();
//         // inner.innerInt = 42;
//     }
// }