using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;

[DefaultExecutionOrder(10)]
public class TitleView : ViewBase<TitleView>
{
    public Button BtnStart;
    public Button BtnExit;

    public GameObject PnlButtons;
    public override IEnumerable<BindDataBase> GetAllBinders()
    {
        MyDebug.Log("TitleView GetAllBinders");
        yield return MyFSM.GetBindState(EGameState.Title)
            .OnEnter(() => PnlButtons.SetActive(true))
            .OnExit(() => PnlButtons.SetActive(false));
    }
}