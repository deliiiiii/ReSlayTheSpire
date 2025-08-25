using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee;

public class TestUpdate : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 1;
    }

    void Update()
    {
        MyDebug.Log($"Update {t1}");
    }

    float t1 => Time.time;
    float t2 => Time.realtimeSinceStartup;

    void FixedUpdate()
    {
        MyDebug.Log($"FixedUpdate {t1}");
    }

}