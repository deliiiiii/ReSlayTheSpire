using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    public class BoxModelManager : MonoBehaviour
    {
        void Awake()
        {
            MapModel.OnAddBox += SpawnBox;
            MapModel.OnRemoveBox += DestroyBox;
            MapModel.OnInputEnd += ShowSprite;
        }
        static readonly Dictionary<Vector2Int, BoxModel> boxModelDic = new ();
        #region Event
        void SpawnBox(Vector2Int vector2Int, BoxData fBoxData)
        {
            // TODO 对象池
            var boxGO = new GameObject($"Box {vector2Int.x} {vector2Int.y}");
            boxGO.transform.SetParent(transform);
            boxGO.transform.position = new Vector3(vector2Int.x, vector2Int.y, 0);
            
            var boxRenderer = boxGO.AddComponent<SpriteRenderer>();
            boxRenderer.sprite = fBoxData.Sprite;
            boxRenderer.enabled = Configer.Instance.SettingsConfig.ShowBoxWhenCreated;
            
            var boxModel = boxGO.AddComponent<BoxModel>();
            boxModel.BoxData = fBoxData;
            
            boxModelDic.Add(vector2Int, boxModel);
        }
        
        void DestroyBox(Vector2Int vector2Int)
        {
            // TODO 对象池
            Destroy(boxModelDic[vector2Int].gameObject);
            boxModelDic.Remove(vector2Int);
        }
        void ShowSprite(Vector2Int vector2Int)
        {
            boxModelDic[vector2Int].GetComponent<SpriteRenderer>().enabled = true;
        }
        #endregion
    }
}