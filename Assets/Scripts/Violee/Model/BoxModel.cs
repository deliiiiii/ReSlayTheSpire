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
    public class BoxModel : MonoBehaviour
    {
        void Awake()
        {
            boxParent ??= new GameObject("BoxParent").transform;
            MapModel.OnInputEnd += ShowSprite;
        }

        [ShowInInspector]
        BoxData boxData;
        static Transform boxParent;
        // TODO 输入方向，输出可以
        // static Dictionary<byte, List<byte>> fromTosDic;
        static Dictionary<Loc, BoxModel> boxModelDic = new ();
        static bool showByDefault = false;


        void ShowSprite(Loc loc)
        {
            boxModelDic[loc].GetComponent<SpriteRenderer>().enabled = true;
        }
        
        public static void OnCreateBoxData(Loc loc, BoxData fBoxData)
        {
            // TODO 对象池
            var boxGO = new GameObject($"Box {loc.X} {loc.Y}");
            boxGO.transform.SetParent(boxParent);
            boxGO.transform.position = new Vector3(loc.X, loc.Y, 0);
            
            var boxRenderer = boxGO.AddComponent<SpriteRenderer>();
            boxRenderer.sprite = fBoxData.Sprite;
            boxRenderer.enabled = showByDefault;
            
            var boxModel = boxGO.AddComponent<BoxModel>();
            boxModel.boxData = fBoxData;
            
            boxModelDic.Add(loc, boxModel);
        }
        public static void OnDestroyBoxData(Loc loc)
        {
            // TODO 对象池
            Destroy(boxModelDic[loc].gameObject);
            boxModelDic.Remove(loc);
        }
    }
}