using System.Collections.Generic;

namespace RSTS;

public class ChooseNameView : Singleton<ChooseNameView>
{
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister<EGameState>(() => GameStateBinders().BindAll());
        MyFSM.OnRelease<EGameState>(() => GameStateBinders().UnBindAll());
    }

    IEnumerable<BindDataBase> GameStateBinders()
    {
        yield return MyFSM.GetBindState(EGameState.ChoosePlayer)
            .OnEnter(() => MyDebug.Log("ChooseNameView Show"))
            .OnUpdate(_ => MyDebug.Log("ChooseNameView Update"));
    }
}