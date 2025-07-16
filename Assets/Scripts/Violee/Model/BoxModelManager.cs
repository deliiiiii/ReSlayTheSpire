using System.Collections.Generic;
using UnityEngine;

namespace Violee
{
    public class BoxModelManager : MonoBehaviour
    {
        void Awake()
        {
            InitWallPrefabDic();
            
            MapModel.OnAddBox += SpawnBox2D;
            MapModel.OnRemoveBox += DestroyBox;
            MapModel.OnInputEnd += ShowSprite;
        }

        public GameObject BoxPrefab;
        Dictionary<byte, GameObject> wallPrefabDic;
        Transform twoDParent;
        Transform TwoDParent
        {
            get
            {
                if(twoDParent == null)
                {
                    twoDParent = new GameObject("2D Box Parent").transform;
                    twoDParent.transform.parent = transform;
                }
                return twoDParent;
            }
        }
        Transform threeDParent;
        Transform ThreeDParent
        {
            get
            {
                if(threeDParent == null)
                {
                    threeDParent = new GameObject("3D Box Parent").transform;
                    threeDParent.transform.parent = transform;
                }
                return threeDParent;
            }
        }
        static readonly Dictionary<Vector2Int, BoxModel> boxModel2DDic = new ();
        static readonly Dictionary<Vector3, BoxModel> boxModel3DDic = new ();
        
        static Vector3 Pos2DTo3D(Vector2Int pos2D) => new Vector3(pos2D.x * 10, 0, pos2D.y * 10);

        #region Prefab

        void InitWallPrefabDic()
        {
            wallPrefabDic = new Dictionary<byte, GameObject>();
            for (int i = 0; i < BoxPrefab.transform.childCount; i++)
            {
                var wallGo = BoxPrefab.transform.GetChild(i).gameObject;
                wallPrefabDic.Add((byte)(1 << i), wallGo);
            }
        }
        #endregion
        #region Event
        void SpawnBox2D(Vector2Int pos2D, BoxData fBoxData)
        {
            // TODO 对象池
            var boxGo = new GameObject($"Box {pos2D.x} {pos2D.y}");
            boxGo.transform.SetParent(TwoDParent);
            boxGo.transform.position = new Vector3(pos2D.x, pos2D.y, 0);
            
            var boxRenderer = boxGo.AddComponent<SpriteRenderer>();
            boxRenderer.sprite = fBoxData.Sprite;
            boxRenderer.enabled = Configer.Instance.SettingsConfig.ShowBoxWhenCreated;
            
            var boxModel = boxGo.AddComponent<BoxModel>();
            boxModel.BoxData = fBoxData;
            
            boxModel2DDic.Add(pos2D, boxModel);
            
            SpawnBox3D(Pos2DTo3D(pos2D), fBoxData);
        }

        void SpawnBox3D(Vector3 pos3D, BoxData fBoxData)
        {
            var boxGo = Instantiate(BoxPrefab, pos3D, Quaternion.identity, ThreeDParent);
            boxGo.name = $"Box {pos3D.x} {pos3D.y}";
            for (int i = 0; i < 8; i++)
            {
                if(fBoxData.HasIthWall(i))
                    boxGo.transform.GetChild(i).gameObject.SetActive(true);
            }
            
            var boxModel = boxGo.AddComponent<BoxModel>();
            boxModel.BoxData = fBoxData;
            boxModel3DDic.Add(pos3D, boxModel);
        }
        
        void DestroyBox(Vector2Int pos2D)
        {
            // TODO 对象池
            var pos3D = Pos2DTo3D(pos2D);
            Destroy(boxModel2DDic[pos2D].gameObject);
            Destroy(boxModel3DDic[pos3D].gameObject);
            boxModel2DDic.Remove(pos2D);
            boxModel3DDic.Remove(pos3D);
        }
        void ShowSprite(Vector2Int vector2Int)
        {
            boxModel2DDic[vector2Int].GetComponent<SpriteRenderer>().enabled = true;
        }
        #endregion
    }
}