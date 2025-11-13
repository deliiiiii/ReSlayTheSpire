using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
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

            RefreshCurPointBuff();
        };
        
        // TODO BindState is deleted
        // GameState.TitleState
        //     .OnUpdate(_ => TickOnTitle());
        // GameState.PlayingState
        //     .OnEnter(OnEnterPlaying)
        //     .OnExit(OnExitPlaying);
    }
    public static void RefreshCurPointBuff()
    {
        // BuffManager.RefreshConBuffs(PlayerCurPoint.Value?.ConnectedPointItems() ?? []);
    }
    
    
    static event Action<BoxPointData?>? OnPlayerExit;
    static event Action<BoxPointData?>? OnPlayerEnter;
    // TODO Observable 不再支持class type
    // public static readonly Observable<BoxPointData> PlayerCurPoint 
    //     = new(null!, p => OnPlayerExit?.Invoke(p), p => OnPlayerEnter?.Invoke(p));
    
    static Transform staTransform => Instance.transform;
    static GameObject staGameObject => Instance.gameObject;
    public static void OnDijkstraEnd(Vector3 pos3D)
    {
        staTransform.position = pos3D + Vector3.up * (3f * staTransform.localScale.y);
    }
    // public static void OnEnterPlaying()
    // {
    //     staGameObject.SetActive(true);
    // }
    //
    // public static void OnExitPlaying()
    // {
    //     staGameObject.SetActive(false);
    //     fpc.OnEnterPlayingState();
    //     // 会让BuffManager清空ConsistentBuff，随后View的显示也会清空
    //     PlayerCurPoint.Value = null;
    // }

    // TODO Observable 不再支持class type
    // public static void TickOnTitle()
    // {
    //     if (InteractInfo.Value is StartBoxInteractInfo)
    //     {
    //         HandleClick();
    //     }
    // }
    // public static void TickOnPlaying(bool hasWindowed)
    // {
    //     fpc.enabled = !hasWindowed;
    //     
    //     if ((InteractInfo.Value?.CanUse ?? false) && !hasWindowed)
    //     {
    //         HandleClick();
    //     }
    // }
    //
    // static void HandleClick()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         InteractInfo.Value!.Act();
    //         OnClickInteract?.Invoke(InteractInfo.Value);
    //     }
    // }
    // public static Vector3 GetPos() => staTransform.position;
    //
    //
    // #region SceneItem
    //
    //
    // public static readonly Observable<InteractInfo?> InteractInfo = new(null);
    // public static event Action<InteractInfo>? OnClickInteract;
    //
    // #endregion
}