using System;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class PlayerManager : SingletonCS<PlayerManager>
{
    static readonly PlayerModel playerModel;
    static PlayerData playerData;
    static PlayerManager()
    {
        playerModel = Configer.PlayerModel;
        playerData = playerModel.PlayerData;
    }
    public static MiniItemData Stamina => playerData.Stamina;
    public static MiniItemData Energy => playerData.Energy;
    public static MiniItemData Gloves => playerData.Gloves;
    public static MiniItemData Dice => playerData.Dice;
        

    public static void OnEnterPlaying(Vector3 pos3D)
    {
        playerModel.transform.position = pos3D + Vector3.up * (1.5f * playerModel.transform.position.y);
        playerData = new PlayerData();
        playerModel.gameObject.SetActive(true);
    }
        
    public static void OnExitPlaying()
    {
        playerModel.gameObject.SetActive(false);
    }

    public static void Tick(float dt)
    {
        playerModel.Fpc.Tick();
    }

    public static Vector3 GetPos => playerModel.transform.position;
}