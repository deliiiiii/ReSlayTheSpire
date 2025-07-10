using System;
using System.Collections;
using UnityEngine;

namespace BehaviourTree
{
    public class YieldTest : MonoBehaviour
    {
        void Awake()
        {
            StartCoroutine(CoTest(10));
        }

        IEnumerator CoTest(int i)
        {
            while (i < 100)
            {
                i++;
                MyDebug.Log($"CoTest: {i} T : {Time.realtimeSinceStartup}", LogType.Tick);
                yield return CoTest2(0);
            }
        }

        IEnumerator CoTest2(int j)
        {
            while (j < 2)
            {
                j++;
                MyDebug.Log($"CoTest: {j} T : {Time.realtimeSinceStartup}", LogType.Tick);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}