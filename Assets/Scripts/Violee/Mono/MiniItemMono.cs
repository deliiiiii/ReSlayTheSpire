using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class MiniItemMono : Singleton<MiniItemMono>
{
    [ShowInInspector]
    MiniItemData miniItemData = null!;
    
    public static Observable<int> StaminaCount => Instance.miniItemData.Stamina.Count;
    public static Observable<int> EnergyCount => Instance.miniItemData.Energy.Count;
    public static Observable<int> CreativityCount => Instance.miniItemData.Creativity.Count;
    public static Observable<int> VioleeCount => Instance.miniItemData.Violee.Count;

    protected override void Awake()
    {
        base.Awake();
        GameManager.GeneratingMapState.OnExit(() =>
        {
            miniItemData = new MiniItemData();
        });
    }
}

