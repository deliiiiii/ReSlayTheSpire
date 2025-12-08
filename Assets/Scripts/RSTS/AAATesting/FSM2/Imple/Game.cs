namespace RSTS;


public class GameFSM : FSM2<GameFSM>
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;

    public GameFSM()
    {
        Launch<GameTitle>();
    }
}

public class GameChoosePlayer : GameFSM.IState
{
    public required GameFSM BelongFSM { get; set; }
}

public class GameTitle : GameFSM.IState
{
    public required GameFSM BelongFSM { get; set; }
}

public partial class GameBattle : GameFSM.IState
{
    public required GameFSM BelongFSM { get; set; }
}