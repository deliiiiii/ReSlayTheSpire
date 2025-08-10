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
    // public static int VioleeCount => Instance.mainItemData.Violee.Count;


    public static void Init() => Instance._Init();

    void _Init()
    {
        GameManager.PlayingState.OnUpdate(_ =>
        {
            if (!Configer.SettingsConfig.IsDevelop)
                return;
            if (Input.GetKey(KeyCode.D)
                && Input.GetKey(KeyCode.E)
                && Input.GetKey(KeyCode.L)
                && Input.GetKey(KeyCode.I))
            {
                Instance.mainItemData.Stamina.Count = 2025;
                Instance.mainItemData.Energy.Count = 813;
                Instance.mainItemData.Creativity.Count = 1130;
            }
        });
    }
    
    public static void OnDijkstraEnd()
    {
        Instance.mainItemData = new MainItemData();
        OnChangeVioleT?.Invoke([]);
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
        if (BuffManager.IsWithFridge)
            trueGain += 2;
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

    public static event Action<List<char>>? OnChangeVioleT;
    public static void GainVioleT()
    {
        if (Instance.mainItemData.LetterList.Count == 6)
        {
            MyDebug.Log("<UNK>");
            return;
        }
        List<char> violeT = ['v', 'i', 'o', 'l', 'e', 'T'];
        Func<char, bool> filter = Instance.mainItemData.LetterList.Count == 5 
            ? _ => true
            : c => c != 'T';
        var c = violeT.RandomItem(c => !Instance.mainItemData.LetterList.Contains(c) && filter(c));
        Instance.mainItemData.LetterList.Add(c);
        OnChangeVioleT?.Invoke(Instance.mainItemData.LetterList);
    }

    public static void ExchangeLetter(int id1, int id2)
    {
        var c1 = Instance.mainItemData.LetterList[id1];
        var c2 = Instance.mainItemData.LetterList[id2];
        Instance.mainItemData.LetterList[id1] = c2;
        Instance.mainItemData.LetterList[id2] = c1;
        OnChangeVioleT?.Invoke(Instance.mainItemData.LetterList);
    }
}

