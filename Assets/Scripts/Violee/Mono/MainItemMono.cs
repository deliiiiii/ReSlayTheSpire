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
    
    public static int StaminaCount => Instance.mainItemData.Stamina.Count;
    public static int EnergyCount => Instance.mainItemData.Energy.Count;
    public static int CreativityCount => Instance.mainItemData.Creativity.Count;
    public static int VioleeCount => Instance.mainItemData.Violee.Count;

    public static void OnDijkstraEnd()
    {
        Instance.mainItemData = new MainItemData();
    }

    
    #region Creativity
    public static void CostCreativity(int cost)
    {
        Instance.mainItemData.Creativity.Count -= cost;
    }
    public static int CheckCreativityCost(int initCost)
    {
        var trueCost = initCost;
        if(BuffManager.IsWithLamp)
            trueCost -= 1;
        return trueCost;
    }
    
    public static void GainCreativity(int gain)
    {
        Instance.mainItemData.Creativity.Count += gain;
        BuffManager.TryUseSmallLamp();
    }
    public static int CheckCreativityGain(int initGain)
    {
        var trueGain = initGain;
        if (BuffManager.IsWithSmallLamp)
        {
            trueGain *= 2;
        }
        return trueGain;
    }
    #endregion

    
    #region Stamina
    public static void CostStamina(int cost)
    {
        Instance.mainItemData.Stamina.Count -= cost;
    }
    public static int CheckStaminaCost(int cost)
    {
        var trueCost = cost;
        if (BuffManager.IsWithCooler)
            trueCost -= 1;
        return trueCost;
    }
    public static void GainStamina(int gain)
    {
        Instance.mainItemData.Stamina.Count += gain;
    }

    public static int CheckStaminaGain(int gain)
    {
        var trueGain = gain;
        return trueGain;
    }
    #endregion

    
    #region Energy
    public static void CostEnergy(int cost)
    {
        Instance.mainItemData.Energy.Count -= cost;
    }
    public static int CheckEnergyCost(int initCost)
    {
        var trueCost = initCost;
        if(BuffManager.IsWithRecordPlayer)
            trueCost -= 1;
        return trueCost;
    }
    public static void GainEnergy(int gain)
    {
        Instance.mainItemData.Energy.Count += gain;
    }
    public static int CheckEnergyGain(int gain)
    {
        var trueGain = gain;
        return trueGain;
    }
    #endregion
}

