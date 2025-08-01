using System;
using UnityEngine;

namespace Violee.Interact;

public class InteractCasterReticle : MonoBehaviour
{
    public required Camera Camera;
    public LayerMask TarLayer;

    readonly Observable<InteractReceiver?> curReceiver 
        = new(null, x => x?.ExitInteract(), x => x?.EnterInteract());

    void Awake()
    {
        Binder.Update(_ =>
        {
            var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var newValue = Physics.Raycast(ray, out var hit, BoxHelper.BoxSize, TarLayer) 
                ? hit.collider.gameObject.GetComponent<InteractReceiver>() 
                : null;
            curReceiver.Value = newValue;
            ;
            // MyDebug.Log(curReceiver.Value == null);
        });
    }
}