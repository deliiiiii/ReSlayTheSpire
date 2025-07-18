using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    public class BoxModelManager : MonoBehaviour
    {
        void Awake()
        {
            MapModel.OnAddBox += (pos2D, fBoxData) => SpawnBox3D(BoxHelper.Pos2DTo3D(pos2D), fBoxData);
            MapModel.OnRemoveBox += DestroyBox;
        }

        public BoxModel BoxPrefab;
        static readonly Dictionary<Vector3, BoxModel> boxModel3DDic = new ();
        
        
        #region Event
        void SpawnBox3D(Vector3 pos3D, BoxData fBoxData)
        {
            var boxModel = Instantiate(BoxPrefab, pos3D, Quaternion.identity, transform);
            boxModel.ReadData(fBoxData);
            boxModel3DDic.Add(pos3D, boxModel);
        }
        
        void DestroyBox(Vector2Int pos2D)
        {
            // TODO 对象池
            var pos3D = BoxHelper.Pos2DTo3D(pos2D);
            Destroy(boxModel3DDic[pos3D].gameObject);
            boxModel3DDic.Remove(pos3D);
        }
        #endregion
    }
}