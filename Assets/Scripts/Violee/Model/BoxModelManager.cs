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
            boxModelPool = new ObjectPool<BoxModel>(BoxPrefab, transform);
            MapModel.OnAddBoxAsync += (pos2D, fBoxData) => SpawnBox3D(BoxHelper.Pos2DTo3D(pos2D), fBoxData);
            MapModel.OnRemoveBox += DestroyBox;
        }

        #region Drag In
        public BoxModel BoxPrefab;
        #endregion
        
        static ObjectPool<BoxModel> boxModelPool;
        static Dictionary<Vector3, BoxModel> boxModel3DDic = new ();
        
        #region Event
        async Task SpawnBox3D(Vector3 pos3D, BoxData fBoxData)
        {
            try
            {
                var boxModel = await boxModelPool.MyInstantiate(pos3D);
                boxModel.ReadData(fBoxData);
                boxModel3DDic.Add(pos3D, boxModel);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        }
        
        void DestroyBox(Vector2Int pos2D)
        {
            var pos3D = BoxHelper.Pos2DTo3D(pos2D);
            boxModelPool.MyDestroy(boxModel3DDic[pos3D]);
            boxModel3DDic.Remove(pos3D);
        }
        #endregion
    }
}