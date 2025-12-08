namespace RSTS;


public class Game
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;
    public GameFSM GameFSM;

    public Game()
    {
        GameFSM = new GameFSM{Outer = this};
    }
}

public class GameFSM : FSM2<Game, GameFSM, GameState>;

public abstract class GameState : GameFSM.StateBase
{
    public Game Game => FSM.Outer;
}
public class GameChoosePlayer : GameState;
public class GameTitle : GameState;

public partial class Battle : GameState;