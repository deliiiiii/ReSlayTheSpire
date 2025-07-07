using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public static class AssetDataBaseExt
    {
        // public static void CloseAllWindows()
        // {
        //     Resources.FindObjectsOfTypeAll<BTEditorWindow>().ForEach(w => w.Close());
        // }
        public static void SafeAddSubAsset(Object objectToAdd, Object assetObject)
        {
            if (assetObject == null || objectToAdd == null)
            {
                MyDebug.LogError("AssetDatabaseExtension.SafeAddSubAsset: assetObject or objectToAdd is null");
                return;
            }

            if (AssetDatabase.Contains(objectToAdd))
            {
                // MyDebug.LogError($"AssetDatabaseExtension.SafeAddSubAsset: {assetObject.name} already exists");
                return;
            }

            if (!EditorUtility.IsPersistent(assetObject))
            {
                MyDebug.LogError($"AssetDatabaseExtension.SafeAddSubAsset: {assetObject.name} is NOT persistent");
                return;
            }
            AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
        }
    }
}