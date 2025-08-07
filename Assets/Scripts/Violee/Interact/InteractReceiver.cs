using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Violee;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Outline))]
    
public class InteractReceiver : MonoBehaviour
{
    public Func<InteractInfo?> GetInteractInfo = () => null;
    Outline? outline;
    void Awake()
    {
        if(gameObject.layer != LayerMask.NameToLayer(nameof(InteractReceiver)))
            MyDebug.LogError($"{name} should be on layer {nameof(InteractReceiver)}");
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
        outline.OutlineColor = GetInteractInfo()?.Color ?? Color.white;
    }
}

public class InteractInfo
{
    public required Action Act;
    public required string Description;
    public required Color Color;
}

public class SceneItemInteractInfo : InteractInfo
{
    public required SceneItemData SceneItemData;
}

public class DoorInteractInfo : InteractInfo
{
    public required List<BoxPointData> InsidePointDataList = [];
    public required WallData WallData;
    public required Func<List<DrawConfig>> GetDrawConfigs;
}