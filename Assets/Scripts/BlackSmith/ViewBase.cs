#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class ViewBase
{
    protected static BlackSmith.MainView MainView;
    protected static BlackSmith.UpgradeView UpgradeView;
    void Awake()
    {
        EditorApplication.playModeStateChanged -= OnExitPlayMode;
        EditorApplication.playModeStateChanged += OnExitPlayMode;
    }

    static void OnExitPlayMode(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingPlayMode)
        {
            MainView = null;
            UpgradeView = null;
        }
    }
}

