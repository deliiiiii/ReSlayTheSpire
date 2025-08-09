using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee;

public class PlayerMono : Singleton<PlayerMono>
{
    static FirstPersonController fpc = null!;

    protected override void Awake()
    {
        base.Awake();
        fpc = GetComponent<FirstPersonController>();
        gameObject.SetActive(false);

        OnPlayerEnter += p =>
        {
            if (!(p?.Visited ?? true))
            {
                p.VisitConnected();
                // MyDebug.Log($"First Enter Point!!{p.BelongBox.Pos2D}:{p.Dir}");
            }
            BuffManager.RefreshConBuffs(p.ConnectedPointItems());
        };
    }
    
    public static event Action<BoxPointData>? OnPlayerEnter;
    public static event Action<BoxPointData>? OnPlayerExit;
    public static readonly Observable<BoxPointData> PlayerCurPoint 
        = new(null!, p => OnPlayerExit?.Invoke(p), p => OnPlayerEnter?.Invoke(p));
    
    static Transform staTransform => Instance.transform;
    static GameObject staGameObject => Instance.gameObject;
    public static void OnDijkstraEnd(Vector3 pos3D)
    {
        staTransform.position = pos3D + Vector3.up * (1.5f * staTransform.localScale.y);
    }
    public static void OnEnterPlaying()
    {
        staGameObject.SetActive(true);
    }

    public static void OnExitPlaying()
    {
        staGameObject.SetActive(false);
    }

    public static void Tick(bool hasWindowed)
    {
        fpc.enabled = !hasWindowed;
        if ((InteractInfo.Value?.CanUse ?? false) && Input.GetMouseButtonDown(0) && !hasWindowed)
        {
            InteractInfo.Value.Act();
            OnClickInteract?.Invoke(InteractInfo.Value);
        }
    }

    public static Vector3 GetPos() => staTransform.position;


    #region SceneItem

    public static readonly Observable<InteractInfo?> InteractInfo = new(null);
    public static event Action<InteractInfo>? OnClickInteract;

    #endregion
}