﻿using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
namespace RSTS.CDMV;

[Serializable]
public abstract class ConfigBase : SerializedScriptableObject;

public abstract class ConfigSingle<T> : ConfigBase, IRefSimple
    where T : ConfigSingle<T>, new()
{
    protected virtual void OnEnable()
    {
        RefPoolSingle<T>.Register(() => (this as T)!);
    }
}

[Serializable]
public abstract class ConfigMulti<T> : ConfigBase, IRefMulti
    where T : ConfigMulti<T>, new()
{
    protected virtual void OnEnable()
    {
        RefPoolMulti<T>.RegisterSome(() => (this as T)!, 1);
    }
    
    [OnValueChanged(nameof(OnNameAndIdChanged))] 
    [ValidateInput(nameof(CheckName), "名称不能为空")]
    public string Name = string.Empty;

    [OnValueChanged(nameof(OnNameAndIdChanged))]
    [ValidateInput(nameof(CheckId), "ID不能为空且不能为负数")]
    [ValidateInput(nameof(CheckNameAndIdIdentical), "ID在当前文件夹有重复")]
    public int ID;

    protected abstract string PrefixName { get; }

    protected virtual bool CheckAll()
    {
        return CheckId() && CheckName() && CheckNameAndIdIdentical();
    }
    
    bool CheckId() => ID >= 0;
    bool CheckName() => Name != string.Empty;
    bool CheckNameAndIdIdentical()
    {
#if UNITY_EDITOR
        if (!CheckName())
            return false;
        var thisPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        var directoryName = Path.GetDirectoryName(thisPath);
        // 获取该目录下所有ScriptableObject
        return Directory.GetFiles(directoryName!, "*.asset")
            .Select(path => path.Replace('\\', '/'))
            .Where(path => path != thisPath)
            .All(path => int.Parse(path.Split('_')[1]) != ID);
#endif
    }
    string newName => $"{PrefixName}_{ID}_{Name}.asset";
    
    void OnNameAndIdChanged()
    {
#if UNITY_EDITOR
        if (!CheckAll())
            return;
        UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), newName);
#endif
    }
}