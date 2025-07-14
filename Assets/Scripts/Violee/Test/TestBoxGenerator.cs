using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class TestBoxGenerator : MonoBehaviour
    {
        public int Height = 4;
        public int Width = 6;

        public Box Box;
        async void Awake()
        {
            var textures = await Resourcer.LoadAssetsAsyncByLabel<Texture2D>("BoxFigma");
            MyDebug.Log($"Loaded {textures.Count} textures");

            List<Sprite> sprites = new();
            textures.ForEach(t =>
            {
                sprites.Add(Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f,
                    0,
                    SpriteMeshType.Tight
                ));
            });

            for(int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    var box = new GameObject($"Box {i} {j}");
                    box.transform.SetParent(transform);
                    box.transform.position = new Vector3(j, i, 0);
                    var boxRenderer = box.AddComponent<SpriteRenderer>();
                    boxRenderer.sprite = sprites.RandomItem();
                }
            }

            for (int i = 1; i <= 8; i++)
            {
                MyDebug.Log((i & 3) == 0);
            }
        }
    }
}