using System;
using UnityEngine;
using UnityEngine.Events;

namespace Violee;

public class InteractReceiver : MonoBehaviour
{
    public Func<InteractCb?> InteractCb = () => null;
    Outline outline = null!;
    void Awake()
    {
        if(gameObject.layer != LayerMask.NameToLayer(nameof(InteractReceiver)))
            MyDebug.LogError($"{name} should be on layer {nameof(InteractReceiver)}");
        outline = gameObject.GetOrAddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 10f;
        SetOutline(false);
        outline.GetColor = () => InteractCb()?.Color ?? Color.white;
    }
    
    
    public void SetOutline(bool shown)
        => outline.enabled = shown;
}

public class InteractCb
{
    public required Func<bool> Condition;
    public required Action Cb;
    public required string Description;
    public required Color Color;
}
