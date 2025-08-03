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
    }

    public void SetOutline(bool shown)
    {
        SetOutline(shown, InteractCb()?.Color ?? Color.white);
    }
    void SetOutline(bool shown, Color color)
    {
        outline.enabled = shown;
        outline.OutlineColor = color;
    }
}

public class InteractCb
{
    public required Action Cb;
    public required string Description;
    public required Color Color;
}
