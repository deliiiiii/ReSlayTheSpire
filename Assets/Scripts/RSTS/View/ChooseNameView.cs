using System.Collections.Generic;

namespace RSTS;

public class ChooseNameView : Singleton<ChooseNameView>
{
    protected override void Awake()
    {
        base.Awake();
        MyFSM.OnRegister(GameStateWrap.One, OnRegisterGameState, OnChangeGameState);
    }

    IEnumerable<BindDataBase> OnRegisterGameState(SlotData arg)
    {
        yield break;
    }

    void OnChangeGameState(SlotData _)
    {
        MyFSM.GetBindState(GameStateWrap.One, EGameState.ChoosePlayer)
            .OnEnter(() => MyDebug.Log("ChooseNameView Show"))
            .OnUpdate(_ => MyDebug.Log("ChooseNameView Update"));
    }
}