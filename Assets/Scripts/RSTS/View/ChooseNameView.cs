using System.Collections.Generic;

namespace RSTS;

public class ChooseNameView : ViewBase<ChooseNameView>
{
    public override IEnumerable<BindDataBase> GetAllBinders()
    {
        yield return MyFSM.GetBindState(EGameState.ChoosePlayer)
            .OnEnter(() => MyDebug.Log("ChooseNameView Show"))
            .OnUpdate(_ => MyDebug.Log("ChooseNameView Update"));
    }
}