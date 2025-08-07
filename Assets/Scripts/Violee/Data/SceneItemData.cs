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
    [SerializeReference][ShowIf(nameof(HasCount))] public GameObject? HideAfterRunOut;
    [SerializeReference][ShowIf(nameof(HasCount))] public GameObject? ShowAfterRunOut;
    
    [Header("IsSleep")]
    public bool IsSleep;
    [ShowIf(nameof(IsSleep))] public float SleepTime = 2.89f;
    
    [Header("Camera")]
    [SerializeField] InteractHasCamera? iCamera;
    public bool HasCamera => iCamera != null;
    
    public event Action? OnRunOut;

    public virtual string GetInteractDes()
    {
        var sb = new StringBuilder();
        sb.Append(DesPre);
        if (StaminaCost > 0)
            sb.Append($":消耗{StaminaCost}点体力,\n");
        return sb.ToString();
    }
    public void Use()
    {
        if (HasCount)
        {
            Count--;
            if (Count <= 0)
            {
                OnRunOut?.Invoke();
            }
        }
        UseEffect();
        OnUseEnd();
    }
    public bool CanUse()
    {
        if (HasCount)
            return Count > 0;
        return true;
    }
    public Color DesColor() => this switch
    {
        {StaminaCost : > 0} => Color.blue,
        _ => Color.white,
    };

    protected virtual void UseEffect()
    {
        PlayerManager.StaminaCount.Value -= StaminaCost;
    }

    void OnUseEnd()
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
}


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