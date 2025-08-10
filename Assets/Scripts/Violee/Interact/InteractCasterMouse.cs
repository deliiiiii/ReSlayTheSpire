using System;
using UnityEngine;

namespace Violee;

public class InteractCasterMouse : MonoBehaviour
{
    public LayerMask TarInteractLayer;
    public LayerMask HitWallLayer;
    public required GameObject HitWallObj;
    readonly Observable<InteractReceiver?> lastIr
        = new(null, x => x?.DisableOutline(), x => x?.EnableOutline());

    public void Tick()
    {
        TickHitWall();
        var ray = CameraMono.PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, BoxHelper.BoxSize, TarInteractLayer))
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
    }

    void TickHitWall()
    {
        var ray = CameraMono.PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, BoxHelper.BoxSize) ||
            (HitWallLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
            return;
        HitWallObj.transform.position = hit.point;
    }
}