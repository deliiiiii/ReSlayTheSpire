using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

[Serializable]
public class SceneItemData : DataBase
{
    public SceneItemData CreateNew(HashSet<EBoxDir> dirSet)
    {
        var newModel = GameObject.Instantiate(OriginModel);
        var newData = newModel.Data;
        newData.InsModel = newModel;
        if(IsAir)
            newData.OccupyAirSet = dirSet;
        else
            newData.OccupyFloorSet = dirSet;
        return newData;
    }
    
    
    [NonSerialized][JsonIgnore] public SceneItemModel InsModel = null!;
    [JsonIgnore] public SceneItemModel OriginModel => Configer.SceneItemModelList.SceneItemModels.First(x => x.Data.ID == ID);
    [ShowInInspector] public HashSet<EBoxDir> OccupyFloorSet = [];
    [ShowInInspector] public HashSet<EBoxDir> OccupyAirSet = [];
    // 暂时所有道具都只占一个格子
    // public int OccupyCount = 1;
    public bool IsAir;
    public int ID;
    public int StaminaCost;
    public string DesPre = string.Empty;
    
    [Header("MinimapIcon")]
    public required Sprite MinimapIcon;
    
    [Header("HasCount")]
    public bool HasCount;
    [ShowIf(nameof(HasCount))] public int Count;
    [ShowIf(nameof(HasCount))] public List<GameObject> HideAfterUseList = [];
    [ShowIf(nameof(HasCount))] public List<GameObject> ShowAfterUseList = [];

    [Header("HasSpreadPos")]
    public bool HasSpreadPos;
    [ShowIf(nameof(HasSpreadPos))][Range(0, 1)]
    public float SpreadPossibility = 0.2f;
    [ShowIf(nameof(HasSpreadPos))] public int SpreadMaxCount = 1;
    [ShowIf(nameof(HasSpreadPos))] [SerializeField]
    public SerializableDictionary<Transform, List<SceneMiniItemModel>> SpreadObjectDic = [];

    public MyList<SceneMiniItemModel> HasSpreadObjList = [];
    [Header("IsSleep")]
    public bool IsSleep;
    [ShowIf(nameof(IsSleep))] public float SleepTime = 2.89f;
    
    [Header("Camera")]
    [SerializeField] InteractHasCamera? iCamera;
    
    [Header("Light")]
    [SerializeField] InteractHasLight? iLight;
    public bool HasCamera => iCamera != null;
    
    public virtual void CheckData()
    {
        if (HideAfterUseList.Count < Count)
            LogErrorWith("HideAfterUseList.Count < Count");
        if (ShowAfterUseList.Count < Count)
            LogErrorWith("ShowAfterUseList.Count < Count");
        // if(MinimapIcon == null)
        //     LogErrorWith("MinimapIcon is null");
    }
    public bool IsActive()
    {
        if (HasCount && Count <= 0)
            return false;
        return true;
    }

    public bool CanUse(out string failReason)
    {
        failReason = string.Empty;
        if (MainItemMono.StaminaCount.Value < StaminaCost)
        {
            failReason = $"体力不足{StaminaCost}, 无法查看";
            return false;
        }
        return CanUseInternal(out failReason);
    }
    
    protected virtual bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        return true;
    }
    
    public void Use()
    {
        if (HasCount)
        {
            Count--;
            HideAfterUseList[Count].SetActive(false);
            ShowAfterUseList[Count].SetActive(true);
            // if (Count <= 0)
            // {
            //     OnRunOut?.Invoke();
            // }
        }
        UseEffect();
        OnUseEnd();
    }
    protected virtual void UseEffect()
    {
        MainItemMono.StaminaCount.Value -= StaminaCost;
    }
    protected virtual void OnUseEnd()
    {
        if (HasCamera)
        {
            var transform = InsModel.transform;
            CameraMono.SceneItemVirtualCamera.Follow = transform;
            CameraMono.SceneItemVirtualCamera.LookAt = transform;
            CameraMono.SceneItemVirtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset =
                new Vector3(iCamera!.CameraTransform.localPosition.x * transform.lossyScale.x,
                    iCamera.CameraTransform.localPosition.y * transform.lossyScale.y,
                    iCamera.CameraTransform.localPosition.z * transform.lossyScale.z);
        }

        if (this is IHasConBuff iHasConBuff)
        {
            iHasConBuff.Activated = true;
            PlayerMono.RefreshCurPointBuff();
        }
    }
    public string GetInteractDes()
    {
        var sb = new StringBuilder();
        sb.Append($"{DesPre}:");
        if (StaminaCost > 0)
            sb.Append($"消耗{StaminaCost}点体力,\n");
        sb.Append(GetInteractDesInternal());
        return sb.ToString();
    }
    protected virtual string GetInteractDesInternal()
    {
        return string.Empty;
    }
    public Color DesColor() => this switch
    {
        {StaminaCost : > 0} => Color.blue,
        FoodItemData => Color.cyan,
        _ => Color.white,
    };
    void LogErrorWith(string str)
    {
        MyDebug.LogError($"{InsModel}.Data Has An Error: {str}");
    }
}


// 0 Sofa,
[Serializable]
public class PurpleSceneItemData : SceneItemData
{
    [Header("Purple")]
    public int Energy;

    protected override string GetInteractDesInternal()
    {
        return $"恢复{Energy}点精力";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.EnergyCount.Value += Energy;
    }
}

[Serializable]
public class ClockItemData : SceneItemData
{
    [Header("Clock")] public bool Watched;
    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        if (Watched)
        {
            failReason = "这个时钟已经被凝视了许久";
            return false;
        }
        return true;
    }
}

// 2 BookShelf,
[Serializable]
public class BookShelfItemData : SceneItemData
{
    [Header("BookShelf")]
    public int EnergyCost;
    public int Creativity;

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        if (MainItemMono.EnergyCount.Value < EnergyCost)
        {
            failReason = $"精力不足{EnergyCost}, 无法阅读";
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        return $"和{EnergyCost}点精力,从书中吸收{Creativity}点灵感。";
    }
    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.EnergyCount.Value -= EnergyCost;
        MainItemMono.CreativityCount.Value += Creativity;
    }
}

//3 RecordPlayer
[Serializable]
public class RecordPlayerItemData : SceneItemData, IHasConBuff
{
    [Header("RecordPlayer")]
    public string BuffDes = "和唱片机在同一个连通区域时，开启房门时消耗精力-1点。";
    
    AudioSource audioSource => InsModel.GetComponent<AudioSource>();
    protected override string GetInteractDesInternal()
    {
        return BuffDes;
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        AudioMono.PlayLoop(audioSource, AudioMono.BGMRecordPlayer.RandomItem());
    }

    public EConBuffType conBuffType => EConBuffType.PlayRecord;
    public string GetDes() => BuffDes;
    public bool Activated { get; set; }
}

// 4 Lamp, 5 SmallLamp, 10 AC
[Serializable]
public class ElectricItemData : SceneItemData
{
    [Header("Electric")]
    public int CreativityCost;

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        if(MainItemMono.CreativityCount.Value < CreativityCost)
        {
            failReason = $"灵感不足{CreativityCost}, 无法使用";
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        return $"消耗{CreativityCost}点灵感。";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.CreativityCount.Value -= CreativityCost;
        // TODO buff
    }
}

[Serializable]
public class FoodItemData : SceneItemData
{
    [Header("Food")]
    public int StaminaGain;
    protected override string GetInteractDesInternal()
    {
        return $"恢复{StaminaGain}点体力";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.StaminaCount.Value += StaminaGain;
    }
}