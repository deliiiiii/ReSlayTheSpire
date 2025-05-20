// #if UNITY_EDITOR
// using UnityEditor;
//
//
// public partial class ViewBase
// {
//     // protected static ReSlayTheSpire.Main.MainView MainView;
//     // protected static ReSlayTheSpire.Battle.BattleView BattleView;
//     // protected static MainModel MainModel => GetModel<MainModel>();
//     // protected static BattleModel BattleModel => GetModel<BattleModel>();
//
//     void Awake()
//     {
//         EditorApplication.playModeStateChanged -= OnExitPlayMode;
//         EditorApplication.playModeStateChanged += OnExitPlayMode;
//     }
//
//     void OnExitPlayMode(PlayModeStateChange state)
//     {
//         if(state == PlayModeStateChange.ExitingPlayMode)
//         {
//             // MainView = null;
//             // BattleView = null;
//         }
//     }
// }
//
// #endif