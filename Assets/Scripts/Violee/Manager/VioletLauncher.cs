using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using UnityEngine;

namespace Violee;

public class VioletLauncher : MonoBehaviour
{
    [ShowInInspector] public string GameState => GameManager.GameState;
    [ShowInInspector] public string WindowState => GameManager.WindowState;
    void Awake()
    {
#if UNITY_EDITOR
        Binder.Update(_ => GUIHelper.RequestRepaint());
#endif
        GameManager.Init();
    }

    public EBoxDir D;
    SceneItemConfig c => Configer.SceneItemConfigList.SceneItemConfigs[0];
    [Button]
    public void Test()
    {
        MapManager.FirstBoxModel.Data.CreateSceneItemData(c, new ([D]));
    }
}