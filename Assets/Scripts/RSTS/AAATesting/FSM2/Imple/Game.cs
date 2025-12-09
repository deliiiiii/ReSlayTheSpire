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
public class GameChoosePlayer : GameFSM.IState
{
    public required GameFSM BelongFSM { get; set; }
}
[Serializable]
public class GameTitle : GameFSM.IState
{
    public required GameFSM BelongFSM { get; set; }
}

[Serializable]
public partial class Battle : GameFSM.IState;