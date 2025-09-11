using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Violee;

public class TestSerialization : SerializedMonoBehaviour
{
    [OdinSerialize][NonSerialized]
    public required TestS TestS;
    
    void Awake()
    {
        (new TestA() as ITest).Func();
        // ITest.Func();
    }
}

interface ITest
{
    virtual void Func()
    {
        Debug.Log("ITest");
    }
}

class TestA : ITest
{
    public void Func()
    {
        Debug.Log("TestA");
    }
}