using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Violee;

public interface IHasInteractReceiver
{
    public InteractInfo GetCb();
}

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Outline))]
    
public class InteractReceiver : MonoBehaviour
{
    public Func<InteractInfo> GetInteractInfo = InteractInfo.CreateUnActive;
    Outline? outline;
    void Awake()
    {
        if (gameObject.layer != LayerMask.NameToLayer(nameof(InteractReceiver)))
        {
            MyDebug.LogError($"{name} should be on layer {nameof(InteractReceiver)}");
            gameObject.layer = LayerMask.NameToLayer(nameof(InteractReceiver));
        }
        outline = gameObject.GetOrAddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 10f; 
        DisableOutline();
    }

    public void DisableOutline()
    {
        if (outline == null)
            return;
        outline.enabled = false;
    }
    public void EnableOutline()
    {
        if (outline == null)
            return;
        outline.enabled = true;
        outline.OutlineColor = GetInteractInfo().Color;
    }
}

public class InteractInfo
{
    public bool Active = true;
    public bool CanUse = true;
    public required string Description;
    public required Color Color;
    public required Action Act;

    public static InteractInfo CreateUnActive()
    {
        return new InteractInfo
        {
            Active = false,
            Description = null!,
            Color = Color.black,
            Act = null!,
        };
    }
    public static InteractInfo CreateInvalid(string description)
    {
        return new InteractInfo
        {
            Active = true,
            CanUse = false,
            Description = description,
            Color = Color.red,
            Act = null!,
        };
    }
}

public class SceneItemInteractInfo : InteractInfo
{
    public required SceneItemData SceneItemData;
}

public class DoorInteractInfo : InteractInfo
{
    public required List<BoxPointData> InsidePointDataList;
    public required WallData WallData;
    public required Func<List<DrawConfig>> GetDrawConfigs;
}

public class SceneMiniItemInteractInfo : InteractInfo
{
    public required SceneMiniItemData SceneMiniItemData;
}

public class StartBoxInteractInfo : InteractInfo
{
    
}