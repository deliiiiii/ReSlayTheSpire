using System;
using UnityEngine;

namespace Violee;

public class PlayerManager : SingletonCS<PlayerManager>
{
    static readonly PlayerMono playerMono;
    static PlayerManager()
    {
        playerMono = Configer.playerMono;

        OnPlayerEnter += p =>
        {
            if (!(p?.Visited ?? true))
            {
                p.VisitConnected();
                // MyDebug.Log($"First Enter Point!!{p.BelongBox.Pos2D}:{p.Dir}");
            }

            IsWithRecordPlayer = BuffManager.ContainsBuff(EBuffType.PlayRecord)
                                 && (p?.ConnectedHasRecordPlayer() ?? false);
        };
    }
    
    public static event Action<BoxPointData>? OnPlayerEnter;
    public static event Action<BoxPointData>? OnPlayerExit;
    public static readonly Observable<BoxPointData> PlayerCurPoint 
        = new(null!, p => OnPlayerExit?.Invoke(p), p => OnPlayerEnter?.Invoke(p));
    public static bool IsWithRecordPlayer;

    public static void OnDijkstraEnd(Vector3 pos3D)
    {
        playerMono.transform.position = pos3D + Vector3.up * (1.5f * playerMono.transform.localScale.y);
    }
    public static void OnEnterPlaying()
    {
        playerMono.gameObject.SetActive(true);
    }
        
    public static void OnExitPlaying()
    {   
        playerMono.gameObject.SetActive(false);
    }

    public static void Tick(bool hasWindowed)
    {
        playerMono.Fpc.enabled = !hasWindowed;
        if ((InteractInfo.Value?.CanUse ?? false) && Input.GetMouseButtonDown(0) && !hasWindowed)
        {
            InteractInfo.Value.Act();
            OnClickInteract?.Invoke(InteractInfo.Value);
        }
    }

    public static Vector3 GetPos() => playerMono.transform.position;


    #region SceneItem

    public static readonly Observable<InteractInfo?> InteractInfo = new(null);
    public static event Action<InteractInfo>? OnClickInteract;

    #endregion
}