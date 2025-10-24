using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

public class TitleView : Singleton<TitleView>
{
    public Button BtnStart;
    public Button BtnExit;

    public GameObject PnlButtons;

    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One,
            canUnbind: OnRegisterGameState,
            alwaysBind: OnChangeGameState);
    }

    IEnumerable<BindDataBase> OnRegisterGameState(GameData gameData)
    {
        yield return Binder.FromBtn(BtnStart).To(() =>
        {
            MyFSM.EnterState(GameStateWrap.One, EGameState.Battle);
        });
    }
    void OnChangeGameState(MyFSM<EGameState> fsm, GameData gameData)
    {
        fsm.GetState(EGameState.Title).OnEnter += () => PnlButtons.SetActive(true);
        fsm.GetState(EGameState.Title).OnExit += () => PnlButtons.SetActive(false);
    }
}