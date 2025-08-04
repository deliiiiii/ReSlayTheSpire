using System;
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
        [Header("Singleton's Model")] 
        public PlayerMono PlayerMonoIns = null!;
        public MapMono MapMonoIns = null!;
        public static PlayerMono playerMono => Instance.PlayerMonoIns;
        public static MapMono mapMono => Instance.MapMonoIns;

        [Header("Object Pool")]
        public BoxModel BoxModelIns = null!;
        public static BoxModel BoxModel => Instance.BoxModelIns;
        
        
        [Header("Config")] 
        public required BoxConfigList BoxConfigListIns;
        public required DrawConfigList DrawConfigListIns;
        public required MiniItemConfigList MiniItemConfigListIns;
        public required SceneItemModelList SceneItemModelListIns;
        public required SettingsConfig SettingsConfigIns;
        
        public static BoxConfigList BoxConfigList => Instance.BoxConfigListIns;
        public static DrawConfigList DrawConfigList => Instance.DrawConfigListIns;
        public static MiniItemConfigList MiniItemConfigList => Instance.MiniItemConfigListIns;
        public static SceneItemModelList SceneItemModelList => Instance.SceneItemModelListIns;
        public static SettingsConfig SettingsConfig => Instance.SettingsConfigIns;
        
        public static void Init()
        {
            if (SettingsConfig.RefreshConfigOnAwake)
                Task.FromResult(LoadConfig());
        }

        static async Task LoadConfig()
        {
            BoxConfigList.BoxConfigs = [];
            var textures = await Resourcer.LoadAssetsAsyncByLabel<Texture2D>("BoxFigma");
            foreach (var t in textures)
            {
                var match = Regex.Match(t.name, @"\d+");
                var id = match.Success ? byte.Parse(match.Value) : new byte();
                var boxConfig = new BoxConfig()
                {
                    Name = t.name,
                    Walls = id,
                    Texture2D = t,
                    // 强制刷新所有权重
                    BasicWeight = 100,
                };
                BoxConfigList.BoxConfigs.Add(boxConfig);
            }

            BoxConfigList.BoxConfigs.Sort((x, y) => x.Walls - y.Walls);
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(BoxConfigList);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            Debug.Log("LoadConfig Completed");
        }
    }
}