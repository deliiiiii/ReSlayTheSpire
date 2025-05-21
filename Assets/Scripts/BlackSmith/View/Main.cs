

namespace BlackSmith
{

    
public class Main : ViewBase
{
    public MainView MainViewIns;

    void Awake()
    {
        MainView = MainViewIns;
        MainView.IBL();
    }
}


}
