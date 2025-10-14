using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressableBatchProcessor
{
    const string TargetFolderPath = "Assets/Config/RSTS";
    const string LabelName = "RSTSConfig";

    [MenuItem("Tools/Mark Folder as Addressable with RSTSConfig Label")]
    public static void MarkFolderAsAddressable()
    {
        if (!Directory.Exists(TargetFolderPath))
        {
            Debug.LogError($"文件夹不存在: {TargetFolderPath}");
            return;
        }
        string[] allFiles = Directory.GetFiles(TargetFolderPath, "*.*", SearchOption.AllDirectories);
        List<string> validAssetPaths = new List<string>();
        foreach (string file in allFiles)
        {
            if (!file.EndsWith(".meta"))
            {
                string assetPath = file.Replace("\\", "/");
                if (assetPath.StartsWith(Application.dataPath))
                {
                    assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
                }
                validAssetPaths.Add(assetPath);
            }
        }
        if (validAssetPaths.Count == 0)
        {
            Debug.Log("选择的文件夹中没有找到有效资源文件");
            return;
        }
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings 未找到，请确保Addressable包已安装并初始化");
            return;
        }
        if (!settings.GetLabels().Contains(LabelName)) 
            settings.AddLabel(LabelName);
        int processedCount = 0;
        foreach (string assetPath in validAssetPaths)
        {
            if (MarkSingleAssetAsAddressable(assetPath, settings)) 
                processedCount++;
        }
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        AssetDatabase.SaveAssets();
        Debug.Log($"成功处理 {processedCount} 个文件，标记为Addressable并设置Label为: {LabelName}");
    }

    static bool MarkSingleAssetAsAddressable(string assetPath, AddressableAssetSettings settings)
    {
        try
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"无法获取资源的GUID: {assetPath}");
                return false;
            }
            AddressableAssetEntry entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            if (entry != null)
            {
                string address = Path.GetFileNameWithoutExtension(assetPath);
                entry.address = address;
                entry.labels.Add(LabelName);
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"处理资源时出错 {assetPath}: {e.Message}");
        }

        return false;
    }
}