// #if UNITY_EDITOR
// using UnityEditor;
// #endif
using UnityEngine;

namespace BehaviourTree
{
    public class TreeTest : MonoBehaviour
    {
        public RootNode Root;
        public int TarFrameRate = 10;
        public bool ShowStartTick = true;
        void Start()
        {
            Application.targetFrameRate = TarFrameRate;
            Binder.Update(Tick);
        }
// #if UNITY_EDITOR
//         [Button]
//         void SaveChange()
//         {
//             EditorUtility.SetDirty(this);
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//             
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//         }
// #endif
        void Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            Root.Tick(dt);
        }
    }
}