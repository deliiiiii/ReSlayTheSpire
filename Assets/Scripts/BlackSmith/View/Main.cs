

namespace BlackSmith
{

    
public class Main : ViewBase
{
    public MainView MainViewIns;
    public UpgradeView UpgradeViewIns;
    void Awake()
    {
        MainView = MainViewIns;
        UpgradeView = UpgradeViewIns;
        MainView.IBL();
        UpgradeView.IBL();
    }
}


}
