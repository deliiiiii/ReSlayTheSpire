using UnityEngine;

namespace Violee.Interact;

public class InteractCasterReticle : MonoBehaviour
{
    public LayerMask TarLayer;

    float radius;
    readonly Observable<InteractReceiver?> lastIr
        = new(null, x => x?.DisableOutline(), x => x?.EnableOutline());
    void Awake()
    {
        var playerCamera = CameraMono.PlayerCamera;
        radius = Configer.SettingsConfig.InteractCasterRadius;
        GameManager.PlayingState
            .OnUpdate(_ =>
            {
                var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (GameManager.HasWindow ||
                    !Physics.Raycast(ray, out var hit, radius) || 
                    (TarLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    lastIr.Value = null;
                    PlayerManager.InteractInfo = null;
                    return;
                }

                var ir = hit.collider.gameObject.GetComponent<InteractReceiver>();
                if (ir == null || ir.GetInteractInfo() == null)
                {
                    lastIr.Value = null;
                    PlayerManager.InteractInfo = null;
                    return;
                }
                lastIr.Value = ir;
                PlayerManager.InteractInfo = lastIr.Value?.GetInteractInfo();
            });
    }
}