using UnityEngine.Events;
using UnityEngine.UI;

public class BindDataBtn
{
    public BindDataBtn(Button btn)
    {
        this.btn = btn;
    }
    Button btn;

    UnityAction act;
    public BindDataBtn SingleTo(UnityAction act)
    {
        UnBindAll();
        this.act = act;
        btn.onClick.AddListener(act);
        return this;
    }

    public BindDataBtn AnotherTo(UnityAction act)
    {
        this.act = act;
        btn.onClick.AddListener(act);
        return this;
    }

    void UnBindAll()
    {
        btn.onClick.RemoveAllListeners();
    }
}