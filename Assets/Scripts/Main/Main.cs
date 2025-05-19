public class Main : ViewBase
{
    public MainView MainViewIns;
    public BattleView BattleViewIns;
    void Awake()
    {
        MainView = MainViewIns;
        BattleView = BattleViewIns;
        MainView.IBL();
    }
}