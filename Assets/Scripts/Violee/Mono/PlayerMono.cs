using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee;

public class PlayerMono : Singleton<PlayerMono>
{
    [NonSerialized][ShowInInspector]
    public PlayerData PlayerData = null!;
    [HideInInspector]
    public FirstPersonController Fpc = null!;

    protected override void Awake()
    {
        Fpc = GetComponent<FirstPersonController>();
        gameObject.SetActive(false);
    }
}