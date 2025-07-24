using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Violee
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        public ObjectPool(T tPrefab, Transform transform, int initCount = 36)
        {
            this.tPrefab = tPrefab;
            this.initCount = initCount;
            objParent = transform;
            _ = MyCreateNew(initCount);
        }
        T tPrefab;
        Transform objParent;
        readonly int initCount;
        readonly Stack<T> availableObject = new();
        int poolCount => availableObject.Count;
        async Task MyCreateNew(int newCount)
        {
            try
            {
                for (int i = 0; i < newCount; i++)
                {
                    if (!Application.isPlaying)
                        return;
                    var g = Object.Instantiate(tPrefab, tPrefab.transform.position, tPrefab.transform.rotation, objParent);
                    g.gameObject.SetActive(false);
                    availableObject.Push(g);
                    await Configer.SettingsConfig.YieldFrames(1/3.28f);
                }
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }

        public async Task<T> MyInstantiate()
        {
            try
            {
                if (availableObject.Count == 0)
                {
                    await MyCreateNew((int)(poolCount == 0 ? initCount : poolCount * 0.5f));
                }
                var g = availableObject.Pop();
                g.transform.SetParent(objParent, worldPositionStays: false);
                g.gameObject.SetActive(true);
                return g;
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }
        public async Task<T> MyInstantiate(Vector3 fPos)
        {
            try
            {
                if (availableObject.Count == 0)
                {
                    await MyCreateNew((int)(poolCount == 0 ? initCount : poolCount * 0.5f));
                }
                var g = availableObject.Pop();
                g.transform.position = fPos;
                g.transform.SetParent(objParent, worldPositionStays: false);
                g.gameObject.SetActive(true);
                return g;
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }
        public async Task<T> MyInstantiate(Vector3 fPos, Quaternion fRot)
        {
            try
            {
                if (availableObject.Count == 0)
                {
                    await MyCreateNew((int)(poolCount == 0 ? initCount : poolCount * 0.5f));
                }
                var g = availableObject.Pop();
                g.transform.position = fPos;
                g.transform.rotation = fRot;
                g.transform.SetParent(objParent, worldPositionStays: false);
                g.gameObject.SetActive(true);
                return g;
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }
        
        public void MyDestroy(T obj)
        {
            if (obj == null)
            {
                MyDebug.LogError("MyDestroy null " + tPrefab.name + " !");
                return;
            }
            obj.gameObject.SetActive(false);
            availableObject.Push(obj);
        }
    }

}
