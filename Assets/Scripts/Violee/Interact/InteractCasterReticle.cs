using UnityEngine;

namespace Violee;

public class InteractCasterReticle : MonoBehaviour
{
    public LayerMask TarLayer;

    readonly Observable<InteractReceiver?> lastIr
        = new(null, x => x?.DisableOutline(), x => x?.EnableOutline());
    void Awake()
    {
        var playerCamera = CameraMono.PlayerCamera;
        var radius = Configer.SettingsConfig.InteractCasterRadius;
        GameManager.PlayingState
            .OnUpdate(_ =>
            {
                var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (GameManager.HasWindow)
                {
                    lastIr.Value = null;
                    return;
                }
                if (!Physics.Raycast(ray, out var hit, radius) || 
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