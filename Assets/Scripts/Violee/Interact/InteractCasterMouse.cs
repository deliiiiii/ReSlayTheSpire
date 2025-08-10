using System;
using UnityEngine;

namespace Violee;

public class InteractCasterMouse : MonoBehaviour
{
    public LayerMask TarLayer;
    readonly Observable<InteractReceiver?> lastIr
        = new(null, x => x?.DisableOutline(), x => x?.EnableOutline());

    public void Init()
    {
        var playerCamera = CameraMono.PlayerCamera;
        GameManager.TitleState
        .OnUpdate(_ =>
        {
            var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, BoxHelper.BoxSize) ||
                (TarLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
            {
                lastIr.Value = null;
                PlayerMono.InteractInfo.Value = null;
                return;
            }
            var ir = hit.collider.gameObject.GetComponent<InteractReceiver>();
            if (ir == null || !ir.GetInteractInfo().Active)
            {
                lastIr.Value = null;
                PlayerMono.InteractInfo.Value = null;
                return;
            }

            lastIr.Value = ir;
            PlayerMono.InteractInfo.Value = lastIr.Value?.GetInteractInfo();
        });
    }
}