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
        GameManager.PlayingState.OnUpdate(_ =>
        {
            if (GameManager.IsWatchingUI)
            {
                lastIr.Value = null;
                PlayerManager.CurInteractCb = null;
                return;
            }
            var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (!Physics.Raycast(ray, out var hit, radius) || 
                (TarLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
            {
                lastIr.Value = null;
                PlayerManager.CurInteractCb = null;
                return;
            }

            var ir = hit.collider.gameObject.GetComponent<InteractReceiver>();
            if (ir?.InteractCb == null)
            {
                lastIr.Value = null;
                PlayerManager.CurInteractCb = null;
                return;
            }
            lastIr.Value = ir;
            PlayerManager.CurInteractCb = lastIr.Value?.InteractCb();
        });
    }

    // void Update()
    // {
    //     var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //     var newValue = Physics.Raycast(ray, out var hit, BoxHelper.BoxSize, TarLayer) 
    //         ? hit.collider.gameObject.GetComponent<InteractReceiver>() 
    //         : null;
    //     curReceiver.Value = newValue;
    // }
}