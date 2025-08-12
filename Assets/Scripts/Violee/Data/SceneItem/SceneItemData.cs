using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    [Header("HasSpreadPos")]
    public bool HasSpreadPos;

    bool OnlyHasSpread => !IsActive && HasSpreadPos;
    [ShowIf(nameof(OnlyHasSpread))] public string SpreadDes = string.Empty;
    [ShowIf(nameof(HasSpreadPos))][Range(0, 1)] public float SpreadPossibility = 0.2f;
    [ShowIf(nameof(HasSpreadPos))] public int SpreadMaxCount = 1;
    [ShowIf(nameof(HasSpreadPos))] [SerializeField] public SerializableDictionary<Transform, List<SceneMiniItemModel>> SpreadObjectDic = [];
    [ShowIf(nameof(HasSpreadPos))] public MyList<SceneMiniItemModel> HasSpreadObjList = [];
    
    public bool IsActive;
    [ShowIf(nameof(IsActive))] public BuffedInt StaminaCost = new(1);
    [ShowIf(nameof(IsActive))] public string DesPre = string.Empty;
    [Header("HasCount")]
    [ShowIf(nameof(IsActive))] public bool HasCount;
    bool OnlyHasCount => IsActive && HasCount;
    [ShowIf(nameof(OnlyHasCount))] public int Count;
    [ShowIf(nameof(OnlyHasCount))] public List<GameObject> HideAfterUseList = [];
    [ShowIf(nameof(OnlyHasCount))] public List<GameObject> ShowAfterUseList = [];
    [ShowIf(nameof(OnlyHasCount))] public string RunOutDes = string.Empty;
    [ShowIf(nameof(OnlyHasCount))] public bool HasDebuff;
    bool OnlyHasDebuff => OnlyHasCount && HasDebuff;
    [ShowIf(nameof(OnlyHasDebuff))] public string RunOutEffectDes = string.Empty;

    
    [Header("HasConBuff")]
    public bool HasConBuff;
    bool OnlyHasConBuff => IsActive && HasConBuff;
    [ShowIf(nameof(OnlyHasConBuff))] [ReadOnly] public ObservableBool ConBuffActivated = new(false);
    [ShowIf(nameof(OnlyHasConBuff))] [SerializeReference] public ConsistentBuffData ConBuffData = null!;
    
    [Header("IsSleep")]
    [ShowIf(nameof(IsActive))]public bool IsSleep;
    bool OnlyIsSleep => IsActive && IsSleep;
    [ShowIf(nameof(OnlyIsSleep))] public float SleepTime = 2.89f;
    
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
            if (Count <= 0)
            {
                OnRunOut();
            }
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

    protected virtual void OnRunOut()
    {
        
    }
    public string GetInteractDes()
    {
        var sb = new StringBuilder();
        if (OnlyHasSpread)
        {
            sb.Append(SpreadDes);
            return sb.ToString();
        }
        sb.Append($"{DesPre}:");
        if (StaminaCost > 0)
            sb.Append($"消耗{StaminaCost}点体力,\n");
        sb.Append(GetInteractDesInternal());
        if (IsActive && HasCount)
        {
            if(Count >= 2)
                sb.Append($"(可用{Count}次)。");
            if(HasDebuff)
                sb.Append($"\n用完后，{GetDebuffDesInternal()}");
        }
        
        if (HasConBuff)
            sb.Append($"\n{ConBuffData.Des}。");
        return sb.ToString();
    }
    protected virtual string GetInteractDesInternal()
    {
        return string.Empty;
    }
    protected virtual string GetDebuffDesInternal()
    {
        return string.Empty;
    }
    public virtual Color DesColor() => this switch
    {
        {StaminaCost.Value : > 0} => Color.blue,
        // FoodItemData => Color.cyan, 
        _ => Color.white,
    };

    public virtual bool ShouldShowMiniIcon()
    {
        return (!IsActive && HasSpreadObjList.Count != 0) 
               || (HasCount && Count > 0)
               || (HasConBuff && ConBuffActivated && (!ConBuffData.HasCount || ConBuffData.Count > 0));
    }
    
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

    protected override string GetDebuffDesInternal()
    {
        return $"灵感-2";
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        MainItemMono.GainEnergy(Energy);
    }

    protected override void OnRunOut()
    {
        base.OnRunOut();
        if (HasDebuff)
        {
            MainItemMono.CostCreativity(2);
        }
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

    public override bool ShouldShowMiniIcon()
    {
        return !Watched;
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
        AudioMono.PlayLoop(audioSource, AudioMono.GetRandomClips);
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

    
    Func<int>? getStaminaCost;
    Func<int> getEnergyCost = null!;
    Func<int> getCreativityCost = null!;
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
        getStaminaCost = () => CatchCostDic[ECatchType.Stamina];
        getEnergyCost = () => MainItemMono.CheckEnergyCost(CatchCostDic[ECatchType.Energy]);
        getCreativityCost = () => MainItemMono.CheckCreativityCost(CatchCostDic[ECatchType.Creativity]);
        
    }

    protected override bool CanUseInternal(out string failReason)
    {
        failReason = string.Empty;
        Func<bool> catchCost = CurCatchType switch
        {
            ECatchType.Stamina => () =>
                MainItemMono.StaminaCount >= StaminaCost + getStaminaCost(),
            ECatchType.Energy => () =>
                MainItemMono.EnergyCount >= getEnergyCost(),
            _ => () => MainItemMono.CreativityCount >= getCreativityCost(),
        };
        if (!catchCost())
        {
            failReason = CurCatchType switch
            {
                ECatchType.Stamina => $"这个捕梦网需要消耗{StaminaCost + getStaminaCost()}点体力",
                ECatchType.Energy => $"这个捕梦网需要消耗{getStaminaCost()}点体力、{getEnergyCost()}点精力",
                _ => $"这个捕梦网需要消耗{getStaminaCost()}点体力、{getCreativityCost()}点灵感",
            };
            return false;
        }
        return true;
    }

    protected override string GetInteractDesInternal()
    {
        if (getStaminaCost == null)
        {
            // 还未绑定Buff
            return "再随机消耗一种属性,抽取1个字母牌";
        }
        var s1 = CurCatchType switch
        {
            ECatchType.Stamina => $"再消耗{getStaminaCost()}点体力",
            ECatchType.Energy => $"再消耗{getEnergyCost()}点精力",
            _ => $"再消耗{getCreativityCost()}点灵感",
        };
        s1 += ",抽取1个字母牌";
        return s1;
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        Action result = CurCatchType switch
        {
            ECatchType.Stamina => () =>
            {
                MainItemMono.CostStamina(getStaminaCost());
            },
            ECatchType.Energy => () =>
            {
                MainItemMono.CostEnergy(getEnergyCost());
            },
            _ => () =>
            {
                MainItemMono.CostCreativity(getCreativityCost());
            },
        };
        result += MainItemMono.GainVioleT;
        result();
    }
}