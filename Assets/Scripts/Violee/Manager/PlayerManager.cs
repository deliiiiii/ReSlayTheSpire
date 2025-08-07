using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class PlayerManager : SingletonCS<PlayerManager>
{
    static readonly PlayerMono playerMono;
    static PlayerData playerData => playerMono.PlayerData;
    static PlayerManager()
    {
        playerMono = Configer.playerMono;
    }
    public static Observable<int> StaminaCount => playerData.Stamina.Count;
    public static Observable<int> EnergyCount => playerData.Energy.Count;
    public static Observable<int> GlovesCount => playerData.Gloves.Count;
    public static Observable<int> DiceCount => playerData.Dice.Count;

    public static void OnDijkstraEnd(Vector3 pos3D)
    {
        playerMono.transform.position = pos3D + Vector3.up * (1.5f * playerMono.transform.localScale.y);
        playerMono.PlayerData = new PlayerData();
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
        if (InteractInfo.Value != null && Input.GetMouseButtonDown(0) && !hasWindowed)
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