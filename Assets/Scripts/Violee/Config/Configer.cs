using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Violee
{
    public class Configer : Singleton<Configer>
    {
        public BoxConfig BoxConfigIns;

        public static List<BoxConfigSingle> BoxConfigList
        {
            get => Instance.BoxConfigIns.BoxConfigList;
            private set => Instance.BoxConfigIns.BoxConfigList = value;
        }
        
        public SettingsConfig SettingsConfigIns;
        public static SettingsConfig SettingsConfig => Instance.SettingsConfigIns;

        protected override void Awake()
        {
            base.Awake();
            if (SettingsConfig.RefreshConfigOnAwake)
                _ = LoadConfig();
            foreach (var boxConfig in BoxConfigList)
            {
                var t = boxConfig.Texture2D;
                boxConfig.Sprite = Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f,
                    0,
                    SpriteMeshType.Tight);
            }
        }

        async Task LoadConfig()
        {
            if (BoxConfigList == null)
                return;
            BoxConfigList = new List<BoxConfigSingle>();
            var textures = await Resourcer.LoadAssetsAsyncByLabel<Texture2D>("BoxFigma");
            foreach (var t in textures)
            {
                var match = Regex.Match(t.name, @"\d+");
                var id = match.Success ? byte.Parse(match.Value) : new byte();
                var boxConfig = new BoxConfigSingle()
                {
                    Name = t.name,
                    Walls = id,
                    Texture2D = t,
                    // 强制刷新所有权重
                    BasicWeight = 100,
                };
                BoxConfigList.Add(boxConfig);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(BoxConfigIns);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            BoxConfigList.Sort((x, y) => x.Walls - y.Walls);
            Debug.Log("LoadConfig Completed");
        }
    }
}