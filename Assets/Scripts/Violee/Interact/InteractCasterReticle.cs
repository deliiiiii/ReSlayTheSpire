using System;
using UnityEngine;

namespace Violee.Interact;

public class InteractCasterReticle : MonoBehaviour
{
    public required Camera Camera;
    public LayerMask TarLayer;

    float radius;
    readonly Observable<InteractReceiver?> lastIr 
        = new(null, x => x?.SetOutline(false), x => x?.SetOutline(true));
    void Awake()
    {
        radius = Configer.SettingsConfig.InteractCasterRadius;
        PlayerManager.InteractStream = this.Bind(() => lastIr.Value?.GetInteractInfo()).ToStream(i => i?.Act());
        GameManager.PlayingState.OnUpdate(_ =>
        {
            var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (GameManager.HasWindow ||
                !Physics.Raycast(ray, out var hit, radius) || 
                (TarLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
            {
                lastIr.Value = null;
                return;
            }

            var ir = hit.collider.gameObject.GetComponent<InteractReceiver>();
            if (ir == null || ir.GetInteractInfo() == null)
            {
                lastIr.Value = null;
                return;
            }
            lastIr.Value = ir;
        });
    }
}