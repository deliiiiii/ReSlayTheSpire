using System;
using UnityEngine;
using UnityEngine.XR;

public class Test : MonoBehaviour
{
    void Awake()
    {
        var h = Tween
            .Linear(3f)
            .Process(x =>
            {
                MyDebug.Log($" T {x}");
            })
            .Finish(() => MyDebug.Log("Teweeen end"));
                            
        Flow.Create()
            .Then(() => MyDebug.Log("F1"))
            .Delay(2f)
            .Then(h)
            .Then(() => MyDebug.Log("Flow End"))
            .Run();
    }
}