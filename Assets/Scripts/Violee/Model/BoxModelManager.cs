using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee
{
    public class BoxModelManager : MonoBehaviour
    {
        void Awake()
        {
            boxModelPool = new ObjectPool<BoxModel>(BoxPrefab, transform, 42);
            MapModel.OnAddBoxAsync += SpawnBox3D;
            MapModel.OnRemoveBox += DestroyBox;
        }

        #region Drag In
        public BoxModel BoxPrefab;
        #endregion
        
        static ObjectPool<BoxModel> boxModelPool;
        static MyKeyedCollection<Vector3, BoxModel> boxModel3DDic = new(b => b.transform.position);
        
        #region Event
        async Task SpawnBox3D(BoxData fBoxData)
        {
            try
            {
                var boxModel = await boxModelPool.MyInstantiate();
                boxModel.ReadData(fBoxData);
                boxModel3DDic.Add(boxModel);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }
        
        void DestroyBox(BoxData fBoxData)
        {
            var pos3D = BoxHelper.Pos2DTo3DBox(fBoxData.Pos2D);
            boxModelPool.MyDestroy(boxModel3DDic[pos3D]);
            boxModel3DDic.Remove(pos3D);
        }
        #endregion
    }
}