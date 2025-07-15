using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    public class Test : MonoBehaviour
    {
        void Awake()
        {
            Dictionary<byte, int> dic = new Dictionary<byte, int>();
            dic.Add(1, 2);
            dic.Add(3, 4);
            dic.Add(5, 6);
            MyDebug.Log("Test end");
            BoxHelper.GetSth();
            MyDebug.Log("Get end");
            
        }
    }
}