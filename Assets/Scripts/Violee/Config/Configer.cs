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
        public SettingsConfig SettingsConfigIns;

        public static BoxConfig BoxConfig => Instance.BoxConfigIns;
        public static SettingsConfig SettingsConfig => Instance.SettingsConfigIns;
        
        async void Start()
        {
            if (SettingsConfig.RefreshConfigOnAwake)
                 await LoadConfig();
        }

        async Task LoadConfig()
        {
            BoxConfig.BoxConfigList = [];
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
                BoxConfig.BoxConfigList.Add(boxConfig);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(BoxConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            BoxConfig.BoxConfigList.Sort((x, y) => x.Walls - y.Walls);
            Debug.Log("LoadConfig Completed");
        }
    }
}