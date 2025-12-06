namespace RSTS;

public static class Main
{
    public static void Bind()
    {
        // Game.OnStateEnter += gameState =>
        // {
        //     switch(gameState) 
        //     {
        //         case GameChoosePlayer gameChoosePlayer:
        //             MyDebug.Log("Entered GameChoosePlayer [View]");
        //             gameState.FSM.EnterState<GameTitle>();
        //             break;
        //         case GameTitle gameTitle:
        //             MyDebug.Log("Entered GameTitle [View]");
        //             gameState.FSM.EnterState<GameBattle>();
        //             break;
        //         case GameBattle gameBattle:
        //             MyDebug.Log("Entered GameBattle [View]");
        //             break;
        //     }
        // };
    }
    public static void Test()
    {
        Game game = new Game();
        game.Launch<Battle>();
    }
}