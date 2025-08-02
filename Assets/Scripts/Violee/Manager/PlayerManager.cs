using System;
using System.Diagnostics.CodeAnalysis;
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
        playerMono.transform.position = pos3D + Vector3.up * (1.5f * playerMono.transform.position.y);
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

    public static void Tick(float dt)
    {
        playerMono.Fpc.Tick();
        if (Input.GetMouseButtonDown(0))
        {
            CurInteractCb?.Cb.Invoke();
            if(CurInteractCb != null)
                OnClickReticle?.Invoke();
        }
    }

    public static Vector3 GetPos => playerMono.transform.position;


    #region SceneItem

    public static InteractCb? CurInteractCb = null;
    public static event Action? OnClickReticle;
    public static void AddEnergy(int added)
    {
        playerData.Energy.Count.Value += added;
    }
    public static void UseStamina(int used)
    {
        playerData.Stamina.Count.Value -= used;
    }
    #endregion
}