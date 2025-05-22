
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace BlackSmith
{

public class MainView : ViewBase
{
    public override void IBL()
    {
        MainModel.Instance.InitData();
    }
}

    
}