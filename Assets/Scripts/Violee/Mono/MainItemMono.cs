using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class MainItemMono : Singleton<MainItemMono>
{
    [ShowInInspector]
    MainItemData mainItemData = null!;
    
    public static Observable<int> StaminaCount => Instance.mainItemData.Stamina.Count;
    public static Observable<int> EnergyCount => Instance.mainItemData.Energy.Count;
    public static Observable<int> CreativityCount => Instance.mainItemData.Creativity.Count;
    public static Observable<int> VioleeCount => Instance.mainItemData.Violee.Count;

    public static void OnDijkstraEnd()
    {
        Instance.mainItemData = new MainItemData();
    }

    public static bool CheckCreativityCost(int initCost, out int trueCost)
    {
        trueCost = initCost;
        return CreativityCount >= initCost;
    }
}

