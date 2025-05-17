using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapNodeBase : ViewBase
{
    public Button BtnEnter;
    
    public virtual void Bind(){}

    public override void IBL()
    {
        Bind();
    }
}
