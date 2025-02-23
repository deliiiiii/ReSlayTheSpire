using UnityEngine;
using QFramework;
using UnityEngine.UI;
public class CounterAppArchitecture : Architecture<CounterAppArchitecture>
{
    protected override void Init()
    {
        RegisterModel(new CounterAppModel());
    }
}


public class CounterAppModel : AbstractModel
{
    public int Count;
    protected override void OnInit()
    {
        Count = 0;
    }
}

public class CounterAppController : MonoBehaviour, IController
{
    [SerializeField]
    Button btnAdd;
    [SerializeField]
    Button btnMinus;
    [SerializeField]
    Text txtCount;

    CounterAppModel model;

    void Awake()
    {
        model = this.GetModel<CounterAppModel>();
        btnAdd.onClick.AddListener(() => {model.Count++;RefreshView();});
        btnMinus.onClick.AddListener(() => {model.Count--;RefreshView();});
        RefreshView();
    }

    void RefreshView()
    {
        txtCount.text = model.Count.ToString();
    }

    public IArchitecture GetArchitecture()
    {
        return CounterAppArchitecture.Interface;
    }
}
