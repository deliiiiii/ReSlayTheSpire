using System;
using UnityEngine;

namespace Test
{
    public class TestDele : MonoBehaviour
    {
        

        void Awake()
        {
            ClassProducer.CreateClass1().Act?.Invoke();
        }
    }

    public class Class1
    {
        public Action Act;
    }

    public static class ClassProducer
    {
        static readonly Action act1 = () => MyDebug.Log($"Act1");
        public static Class1 CreateClass1()
        {
            var ret = new Class1()
            {
                Act = act1,
            };
            return ret;
        }
    }
}