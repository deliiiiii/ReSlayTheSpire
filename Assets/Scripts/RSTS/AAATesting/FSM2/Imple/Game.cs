using System;

namespace RSTS;
[Serializable]
public class GameFSM : FSM2<GameFSM>
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;

    public GameFSM()
    {
        Launch<GameTitle>();
    }
}

[Serializable]
public class GameChoosePlayer : FSMState<GameFSM, GameChoosePlayer>;
[Serializable]
public class GameTitle : FSMState<GameFSM, GameTitle>;
[Serializable]
public partial class Battle : FSMState<GameFSM, Battle>;
