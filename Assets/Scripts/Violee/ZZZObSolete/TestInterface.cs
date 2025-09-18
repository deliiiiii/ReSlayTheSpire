// using UnityEngine;
//
// public class TestInterface : MonoBehaviour
// {
//     void Awake()
//     {
//         ITest<B>.Func3();
//         (new TestA<B>() as ITest<B>).Func();
//         // ITest.Func2();
//     }
// }
//
// interface ITest<T> where T : ITest<T>
// {
//     void Func()
//     {
//         Debug.Log("ITest");
//     }
//     
//     static virtual void Func2()
//     {
//         Debug.Log("ITest2");
//     }
//     
//     static void Func3()
//     {
//         T.Func2();
//     }
// }
//
// class TestA<T> : ITest<T> where T : ITest<T>
// {
//     public static void Func()
//     {
//         Debug.Log("TestA");
//     }
// }
//
// class B : ITest<B>;