using UnityEngine;

namespace BlackSmith;


public class Main : ViewBase
{
    public required MainView MainViewIns;
    public required UpgradeView UpgradeViewIns;
    void Awake()
    {
        MainView = MainViewIns;
        UpgradeView = UpgradeViewIns;
        MainView.IBL();
        UpgradeView.IBL();
    }
}