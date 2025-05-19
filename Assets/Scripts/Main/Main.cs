using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Main : ViewBase
{
    void Update()
    {
        foreach (var pair in Binder.UpdateDic)
        {
            foreach (var bindDataUpdate in pair.Value)
            {
                bindDataUpdate.Act();
            }
        }
    }
}

public partial class Main
{
    public MainView MainViewIns;
    public BattleView BattleViewIns;
    void Awake()
    {
        MainView = MainViewIns;
        BattleView = BattleViewIns;
        MainView.IBL();

        Binder.Update(() => MyDebug.Log(2), EUpdatePri.P2);
        Binder.Update(() => MyDebug.Log(0), EUpdatePri.Default);
        Binder.Update(() => MyDebug.Log(1), EUpdatePri.P1).UnBind();
        }
}