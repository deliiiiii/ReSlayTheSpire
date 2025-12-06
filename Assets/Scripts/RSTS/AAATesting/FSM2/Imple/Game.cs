namespace RSTS;


public class Game
{
    public string PlayerName = "DELI";
    public bool HasLastBuff;
    GameFSM gameFSM;

    public Game()
    {
        gameFSM = new GameFSM{Outer = this};
    }
    public void Launch<T>() where T : GameState => gameFSM.Launch<T>();
}

public class GameFSM : FSM2<Game, GameFSM, GameState>;
public abstract class GameState : StateBase<GameFSM>;
public class GameChoosePlayer : GameState;
public class GameTitle : GameState;

public partial class Battle : GameState;