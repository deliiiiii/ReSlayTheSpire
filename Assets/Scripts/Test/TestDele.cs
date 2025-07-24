using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class IsExternalInit : MonoBehaviour
    {
        public int TestPro { get; set; }

        public event Action? EventTest;

        void Awake()
        {
            EventTest?.Invoke();
            EventTest += () => Debug.Log("Test");
            EventTest.Invoke();
        }
    }
    
    public record TestRecord(int i, List<int> l);
}

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}