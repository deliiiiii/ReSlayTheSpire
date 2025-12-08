namespace RSTS;

public static class Main
{
    public static void Bind()
    {
        GameFSM.OnStateEnter += gameState =>
        {
            switch(gameState) 
            {
                case GameChoosePlayer gameChoosePlayer:
                    MyDebug.Log("Entered GameChoosePlayer [View]");
                    break;
                case GameTitle gameTitle:
                    MyDebug.Log("Entered GameTitle [View]");
                    break;
                case GameBattle battle:
                    MyDebug.Log("Entered Battle [View]");
                    break;
            }
        };
    }
    public static void Test()
    {
        GameFSM gameFSM = new ();
    }
}