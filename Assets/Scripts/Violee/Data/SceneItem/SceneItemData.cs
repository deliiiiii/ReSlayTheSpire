using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        newData.BindBuff();
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
    public BuffedInt StaminaCost = new(1);
    public string DesPre = string.Empty;
    
    [Header("MinimapIcon")]
    public required Sprite MinimapIcon;

    [Header("HasSpreadPos")]
    public bool HasSpreadPos;
    [ShowIf(nameof(HasSpreadPos))][Range(0, 1)] public float SpreadPossibility = 0.2f;
    [ShowIf(nameof(HasSpreadPos))] public int SpreadMaxCount = 1;
    [ShowIf(nameof(HasSpreadPos))] [SerializeField] public SerializableDictionary<Transform, List<SceneMiniItemModel>> SpreadObjectDic = [];
    [ShowIf(nameof(HasSpreadPos))] public MyList<SceneMiniItemModel> HasSpreadObjList = [];
    
    public bool IsActive;
    [Header("HasCount")]
    [ShowIf(nameof(IsActive))] public bool HasCount;
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasCount))] public int Count;
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasCount))] public List<GameObject> HideAfterUseList = [];
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasCount))] public List<GameObject> ShowAfterUseList = [];
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasCount))] public string RunOutDes = string.Empty;

    
    [Header("HasConBuff")]
    public bool HasConBuff;
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasConBuff))] [ReadOnly] public ObservableBool ConBuffActivated = new(false);
    [ShowIf(nameof(IsActive))][ShowIf(nameof(HasConBuff))] [SerializeReference] public ConsistentBuffData ConBuffData = null!;
    
    [Header("IsSleep")]
    [ShowIf(nameof(IsActive))]public bool IsSleep;
    [ShowIf(nameof(IsActive))][ShowIf(nameof(IsSleep))] public float SleepTime = 2.89f;
    
    [Header("Camera")]
    [ShowIf(nameof(IsActive))][SerializeField] InteractHasCamera? iCamera;
    
    [Header("Light")]
    [ShowIf(nameof(IsActive))][SerializeField] InteractHasLight? iLight;
    public bool HasCamera => iCamera != null;

    protected virtual void BindBuff()
    {
        StaminaCost.SetBuff(MainItemMono.CheckStaminaCost);
        ConBuffActivated.OnValueChangedAfter += _ => PlayerMono.RefreshCurPointBuff();
    }
    
    public virtual void CheckData()
    {
        if (HideAfterUseList.Count < Count)
            LogErrorWith("HideAfterUseList.Count < Count");
        if (ShowAfterUseList.Count < Count)
            LogErrorWith("ShowAfterUseList.Count < Count");
        // if(MinimapIcon == null)
        //     LogErrorWith("MinimapIcon is null");
    }

    public bool CanUse(out string failReason)
    {
        failReason = string.Empty;
        if (HasCount && Count <= 0)
        {
            failReason = RunOutDes;
            return false;
        }
        if (MainItemMono.StaminaCount < StaminaCost)
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
        MainItemMono.CostStamina(StaminaCost);
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

        if (HasConBuff)
        {
            ConBuffActivated.Value = true;
        }
    }
    public string GetInteractDes()
    {
        var sb = new StringBuilder();
        sb.Append($"{DesPre}:");
        if (StaminaCost > 0)
            sb.Append($"消耗{StaminaCost}点体力,\n");
        sb.Append(GetInteractDesInternal());
        if (HasConBuff)
            sb.Append($"\n{ConBuffData.Des}。");
        return sb.ToString();
    }
    protected virtual string GetInteractDesInternal()
    {
        return string.Empty;
    }
    public Color DesColor() => this switch
    {
        {StaminaCost.Value : > 0} => Color.blue,
        // FoodItemData => Color.cyan, 
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
    public BuffedInt Energy = new(1);

    protected override void BindBuff()
    {
        base.BindBuff();
        Energy.SetBuff(MainItemMono.CheckEnergyGain);
    }

    protected override string GetInteractDesInternal()
    {
        return $"恢复{Energy}点精力";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.GainEnergy(Energy);
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
    public BuffedInt EnergyCost = new(0);
    public BuffedInt Creativity = new(0);

    protected override void BindBuff()
    {
        base.BindBuff();
        EnergyCost.SetBuff(MainItemMono.CheckEnergyCost);
        Creativity.SetBuff(MainItemMono.CheckCreativityGain);
    }

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        if (MainItemMono.EnergyCount < EnergyCost)
        {
            failReason = $"精力不足{EnergyCost}, 无法阅读";
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        return $"并消耗{EnergyCost}点精力,从书中吸收{Creativity}点灵感。";
    }
    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.CostEnergy(EnergyCost);
        MainItemMono.GainCreativity(Creativity);
    }
}

//3 RecordPlayer
[Serializable]
public class RecordPlayerItemData : SceneItemData
{
    AudioSource audioSource => InsModel.GetComponent<AudioSource>();
    AudioClip? curClip;

    protected override void UseEffect()
    {
        base.UseEffect();
        PlayOne();
        IsActive = false;
    }

    public void PlayOne()
    {
        curClip = AudioMono.BGMRecordPlayer.RandomItem(x => x != curClip);
        AudioMono.PlayLoop(audioSource, curClip);
    }
}

// 4 Lamp, 5 SmallLamp, 10 AC
[Serializable]
public class ElectricItemData : SceneItemData
{
    [Header("Electric")]
    public BuffedInt CreativityCost = new(0);
    
    protected override void BindBuff()
    {
        base.BindBuff();
        CreativityCost.SetBuff(MainItemMono.CheckCreativityCost);
    }

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        if(MainItemMono.CreativityCount < CreativityCost)
        {
            failReason = $"灵感不足{CreativityCost}, 无法使用";
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        return $"并消耗{CreativityCost}点灵感";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.CostCreativity(CreativityCost);
        IsActive = false;
    }
}

public enum ECatchType
{
    Stamina,
    Energy,
    Creativity,
}

// 14 
[Serializable]
public class DreamCatcherItemData : SceneItemData
{
    [Header("DreamCatcher")]
    [Range(0f, 1f)]
    public float StaminaCosProbability;
    float d1 => 1 - StaminaCosProbability;
    [MinValue(0f)][MaxValue(nameof(d1))]
    public float EnergyProbability;
    [ShowInInspector] public float CreativityProbability => 1f - StaminaCosProbability - EnergyProbability;
    
    [ReadOnly] public ECatchType CurCatchType;
    public SerializableDictionary<ECatchType, GameObject> CatchGoDic = [];
    public SerializableDictionary<ECatchType, int> CatchCostDic = [];


    Func<int> GetStaminaCost = null!;
    Func<int> GetEnergyCost = null!;
    Func<int> GetCreativityCost = null!;
    protected override void BindBuff()
    {
        base.BindBuff();
        Dictionary<ECatchType, int> gos = new()
        {
            {ECatchType.Stamina, (int)(StaminaCosProbability * 100)},
            {ECatchType.Energy, (int)(EnergyProbability * 100)},
            {ECatchType.Creativity, (int)((1 - StaminaCosProbability - EnergyProbability) * 100)}
        };
        CurCatchType = gos.ToList().RandomItem(weightFunc: pair => pair.Value).Key;
        CatchGoDic.ForEach(pair =>
        {
            pair.Value.SetActive(pair.Key == CurCatchType);
        });
        GetStaminaCost = () => MainItemMono.CheckStaminaCost(
            (CurCatchType == ECatchType.Stamina ? CatchCostDic[ECatchType.Stamina] : 0) + StaminaCost);
        GetEnergyCost = () => MainItemMono.CheckEnergyCost(CatchCostDic[ECatchType.Energy]);
        GetCreativityCost = () => MainItemMono.CheckCreativityCost(CatchCostDic[ECatchType.Creativity]);
        
    }

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        Func<bool> catchCost = CurCatchType switch
        {
            ECatchType.Stamina => () =>
                MainItemMono.StaminaCount >= GetStaminaCost(),
            ECatchType.Energy => () =>
                MainItemMono.EnergyCount >= GetEnergyCost(),
            _ => () => MainItemMono.CreativityCount >= GetCreativityCost(),
        };
        if (!catchCost())
        {
            failReason = CurCatchType switch
            {
                ECatchType.Stamina => $"这个捕梦网需要消耗{GetStaminaCost()}点体力",
                ECatchType.Energy => $"这个捕梦网需要消耗{GetStaminaCost()}点体力、{GetEnergyCost()}点精力",
                _ => $"这个捕梦网需要消耗{GetStaminaCost()}点体力、{GetCreativityCost()}点灵感",
            };
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        var s1 = CurCatchType switch
        {
            ECatchType.Stamina => $"再消耗{GetStaminaCost()}点体力",
            ECatchType.Energy => $"再消耗{GetEnergyCost()}点精力",
            _ => $"再消耗{GetCreativityCost()}点灵感",
        };
        s1 += "，转化为1个VioleT";
        return s1;
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        Action result = CurCatchType switch
        {
            ECatchType.Stamina => () =>
            {
                MainItemMono.CostStamina(GetStaminaCost());
            },
            ECatchType.Energy => () =>
            {
                MainItemMono.CostEnergy(GetEnergyCost());
            },
            _ => () =>
            {
                MainItemMono.CostCreativity(GetCreativityCost());
            },
        };
        result += MainItemMono.GainVioleT;
        result();
    }
}