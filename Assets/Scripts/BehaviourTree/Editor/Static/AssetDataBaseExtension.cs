using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public static class AssetDataBaseExtension
    {
        public static void CloseAllWindows()
        {
            Resources.FindObjectsOfTypeAll<BTEditorWindow>().ForEach(w => w.Close());
        }
        public static void SafeAddSubAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
        {
            if (assetObject == null || objectToAdd == null || AssetDatabase.Contains(objectToAdd))
                return;
            AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
        }
    }
}