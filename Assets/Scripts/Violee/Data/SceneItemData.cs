using System;
using System.Collections.Generic;
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
    
    [Header("HasCount")]
    public bool HasCount;

    [ShowIf(nameof(HasCount))] public int Count;
    [SerializeReference][ShowIf(nameof(HasCount))] public List<GameObject> HideAfterUseList = [];
    [SerializeReference][ShowIf(nameof(HasCount))] public List<GameObject> ShowAfterUseList = [];
    
    [Header("IsSleep")]
    public bool IsSleep;
    [ShowIf(nameof(IsSleep))] public float SleepTime = 2.89f;
    
    [Header("Camera")]
    [SerializeField] InteractHasCamera? iCamera;
    public bool HasCamera => iCamera != null;
    
    public virtual void CheckData()
    {
        if (HideAfterUseList.Count < Count)
            LogErrorWith("HideAfterUseList.Count < Count");
        if (ShowAfterUseList.Count < Count)
            LogErrorWith("ShowAfterUseList.Count < Count");
    }
    public bool IsActive()
    {
        if (HasCount && Count <= 0)
            return false;
        return true;
    }

    public virtual bool CanUse(out string failReason)
    {
        failReason = string.Empty;
        if (PlayerManager.StaminaCount.Value < StaminaCost)
        {
            failReason = $"体力不足{StaminaCost}, 无法查看";
            return false;
        }
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
        PlayerManager.StaminaCount.Value -= StaminaCost;
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
    }
    public virtual string GetInteractDes()
    {
        var sb = new StringBuilder();
        sb.Append(DesPre);
        if (StaminaCost > 0)
            sb.Append($":消耗{StaminaCost}点体力,\n");
        return sb.ToString();
    }
    public Color DesColor() => this switch
    {
        {StaminaCost : > 0} => Color.blue,
        _ => Color.white,
    };
    void LogErrorWith(string str)
    {
        MyDebug.LogError($"{InsModel}.Data Has An Error: {str}");
    }
}


// 1 Sofa,
[Serializable]
public class PurpleSceneItemData : SceneItemData
{
    [Header("Purple")]
    public int Energy;
    public override string GetInteractDes()
    {
        var sb = new StringBuilder(base.GetInteractDes());
        sb.Append($"恢复{Energy}点精力");
        return sb.ToString();
    }

    protected override void UseEffect()
    {
        base.UseEffect();
        PlayerManager.EnergyCount.Value += Energy;
    }
}

// 2 BookShelf,
[Serializable]
public class BookShelfItemData : SceneItemData
{
    [Header("BookShelf")]
    public int EnergyCost;
    public int Creativity;

    public override bool CanUse(out string failReason)
    {
        if (!base.CanUse(out failReason))
            return false;
        failReason = string.Empty;
        if (PlayerManager.EnergyCount.Value < EnergyCost)
        {
            failReason = $"精力不足{EnergyCost}, 无法阅读";
            return false;
        }

        return true;
    }

    public override string GetInteractDes()
    {
        var sb = new StringBuilder(base.GetInteractDes());
        sb.Append($"和{EnergyCost}点精力,从书中吸收{Creativity}点灵感。");
        return sb.ToString();
    }
    protected override void UseEffect()
    {
        base.UseEffect();
        PlayerManager.EnergyCount.Value -= EnergyCost;
        PlayerManager.CreativityCount.Value += Creativity;
    }
}