using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor.Callbacks;
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
        Dictionary<Loc, BoxModel> boxModelDic = new ();
        #region Event
        void SpawnBox(Loc loc, BoxData fBoxData)
        {
            // TODO 对象池
            var boxGO = new GameObject($"Box {loc.X} {loc.Y}");
            boxGO.transform.SetParent(transform);
            boxGO.transform.position = new Vector3(loc.X, loc.Y, 0);
            
            var boxRenderer = boxGO.AddComponent<SpriteRenderer>();
            boxRenderer.sprite = fBoxData.Sprite;
            boxRenderer.enabled = SettingsModel.SettingsConfig.ShowBoxWhenCreated;
            
            var boxModel = boxGO.AddComponent<BoxModel>();
            boxModel.BoxData = fBoxData;
            
            boxModelDic.Add(loc, boxModel);
        }
        
        void DestroyBox(Loc loc)
        {
            // TODO 对象池
            Destroy(boxModelDic[loc].gameObject);
            boxModelDic.Remove(loc);
        }
        void ShowSprite(Loc loc)
        {
            boxModelDic[loc].GetComponent<SpriteRenderer>().enabled = true;
        }
        #endregion
    }
}