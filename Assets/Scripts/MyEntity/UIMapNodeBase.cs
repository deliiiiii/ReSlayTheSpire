using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapNodeBase : MonoBehaviour
{
    public Button BtnEnter;

    void Awake()
    {
        BtnEnter.onClick.AddListener(OnClickEnter);
    }

    void OnClickEnter()
    {
        MyEvent.Fire(new OnClickEnterMapNodeEvent(){UIMapNode = this});
    }
}

public class OnClickEnterMapNodeEvent
{
    public UIMapNodeBase UIMapNode;
}
