using System;
using System.Text;
using UnityEngine;

namespace Violee;

[Serializable]
public abstract class SceneMiniItemData : DataBase
{
    public Color InteractColor;
    // public string DesPre = string.Empty;
    public event Action? OnPickedUp;
    public string GetInteractDes()
    {
        var sb = new StringBuilder();
        // sb.Append($"拾{DesPre},");
        sb.Append(GetInteractDesInternal());
        return sb.ToString();
    }

    protected abstract string GetInteractDesInternal();

    public virtual void CheckData()
    {
        
    }
    public void PickUp()
    {
        PickUpInternal();
        OnPickedUp?.Invoke();
    }

    protected abstract void PickUpInternal();
}

[Serializable]
public class SceneMiniItemDataBook : SceneMiniItemData
{
    public int CreativityGain;

    protected override string GetInteractDesInternal()
    {
        return $"看本小书，获得{CreativityGain}点灵感";
    }

    protected override void PickUpInternal()
    {
        MainItemMono.CreativityCount.Value += CreativityGain;
    }
}

[Serializable]
public class SceneMiniItemDataFood : SceneMiniItemData
{
    public int StaminaGain;
    protected override string GetInteractDesInternal()
    {
        return $"获得{StaminaGain}点体力";
    }

    protected override void PickUpInternal()
    {
        MainItemMono.StaminaCount.Value += StaminaGain;
    }
}

[Serializable]
public class SceneMiniItemRecordPlayer : SceneMiniItemData
{
    public required SceneItemModel SceneItemModel;
    protected override string GetInteractDesInternal()
    {
        return $"切下一首歌。";
    }

    protected override void PickUpInternal()
    {
        var data = SceneItemModel.Data as RecordPlayerItemData;
        data?.PlayOne();
    }
}