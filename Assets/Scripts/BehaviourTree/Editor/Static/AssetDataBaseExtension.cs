using UnityEditor;

namespace BehaviourTree
{
    public static class AssetDataBaseExtension
    {
        public static void SafeAddSubAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
        {
            if (assetObject == null || objectToAdd == null || AssetDatabase.Contains(objectToAdd))
                return;
            AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
        }
    }
}