using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Violee;

public class InteractReceiver : MonoBehaviour
{
    public Func<InteractInfo?> GetInteractInfo = () => null;
    Outline outline = null!;
    void Awake()
    {
        if(gameObject.layer != LayerMask.NameToLayer(nameof(InteractReceiver)))
            MyDebug.LogError($"{name} should be on layer {nameof(InteractReceiver)}");
        outline = gameObject.GetOrAddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 10f;
        SetOutline(false);
    }

    public void SetOutline(bool shown)
    {
        SetOutline(shown, GetInteractInfo()?.Color ?? Color.white);
    }
    void SetOutline(bool shown, Color color)
    {
        outline.enabled = shown;
        outline.OutlineColor = color;
    }
}

public class InteractInfo
{
    public required Action Act;
    public required string Description;
    public required Color Color;

    public bool IsSleep;
    public float SleepTime;
    
    public bool IsOpenDoor;
    public WallData WallData = null!;
    public Func<List<DrawConfig>> GetDrawConfigs = () => [];
}