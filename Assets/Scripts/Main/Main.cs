using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Main : ViewBase
{
    public MainView MainViewIns;
    public BattleView BattleViewIns;
    void Awake()
    {
        MainView = MainViewIns;
        BattleView = BattleViewIns;
        MainView.gameObject.SetActive(true);
    }
}