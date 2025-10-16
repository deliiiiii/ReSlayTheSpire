using System.Collections.Generic;

namespace RSTS;

public class ChooseNameView : Singleton<ChooseNameView>
{
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, _ => GameStateBinders().BindAll());
        MyFSM.OnRelease(GameStateWrap.One, _ => GameStateBinders().UnBindAll());
    }

    IEnumerable<BindDataBase> GameStateBinders()
    {
        yield return MyFSM.GetBindState(GameStateWrap.One, EGameState.ChoosePlayer)
            .OnEnter(() => MyDebug.Log("ChooseNameView Show"))
            .OnUpdate(_ => MyDebug.Log("ChooseNameView Update"));
    }
}