using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class TitleView : ViewBase
{
    public Button BtnStart;
    public Button BtnExit;

    public GameObject PnlButtons;
    
    void BindGame(GameData gameData, Func<EGameState, IStateForView> func)
    {
        func(EGameState.Title)
            .OnEnterAfter(() => PnlButtons.SetActive(true))
            .OnExitBefore(() => PnlButtons.SetActive(false));
    }
    IEnumerable<BindDataBase> CanUnbindGame(GameData gameData)
    {
        yield return Binder.FromEvt(BtnStart.onClick).To(() => gameData.EnterState(EGameState.Battle));
    }

    public override void Bind()
    {
        GameData.OnRegister(alwaysBind: BindGame, canUnbind: CanUnbindGame);
    }
}