using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Violee
{
    public class MapModel : MonoBehaviour
    {
        MapData mapData;
        // Dictionary<Loc, BoxData> boxDic;
        public int Height = 4;
        public int Width = 6;
        public SerializableDictionary<int, Sprite> SpriteDic;
        async Task Awake()
        {
            var textures = await Resourcer.LoadAssetsAsyncByLabel<Texture2D>("BoxFigma");
            textures.ForEach(t =>
            {
                var match = Regex.Match(t.name, @"\d+");
                int id = match.Success ? int.Parse(match.Value) : 0;
                SpriteDic.Add(id, Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f,
                    0,
                    SpriteMeshType.Tight
                ));
            });
        }
        public void StartGenerate()
        {
            for(int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    var box = new GameObject($"Box {j} {i}");
                    box.transform.SetParent(transform);
                    box.transform.position = new Vector3(j, i, 0);
                    var boxRenderer = box.AddComponent<SpriteRenderer>();
                    boxRenderer.sprite = SpriteDic.Values.ToList().RandomItem();
                }
            }
        }
    }
}