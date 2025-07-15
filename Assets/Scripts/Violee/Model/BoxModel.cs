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
        [ShowInInspector]
        BoxData boxData;
        
        // 输入方向，输出可以
        static Dictionary<byte, List<byte>> fromTosDic;
        static Transform BoxParent = new GameObject("BoxParent").transform;
        static Dictionary<Loc, BoxModel> boxModelDic = new ();
        
        // public static bool CanGoThroughFromTo(BoxData b, EBoxSide s1, EBoxSide s2)
        // {
        //     return CanGoThroughFromTo(b.Walls, (byte)s1, (byte)s2);
        // }
        public static void OnCreateBoxData(Loc loc, BoxData fBoxData)
        {
            // TODO 对象池
            var boxGO = new GameObject($"Box {loc.X} {loc.Y}");
            boxGO.transform.SetParent(BoxParent);
            boxGO.transform.position = new Vector3(loc.X, loc.Y, 0);
            
            var boxRenderer = boxGO.AddComponent<SpriteRenderer>();
            boxRenderer.sprite = fBoxData.Sprite;
            
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