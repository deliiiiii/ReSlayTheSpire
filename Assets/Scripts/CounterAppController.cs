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
        btnAdd.onClick.AddListener(() => {this.SendCommand<AddCommand>();RefreshView();});
        btnMinus.onClick.AddListener(() => {this.SendCommand<MinusCommand>();RefreshView();});
        RefreshView();


        this.RegisterEvent<OnCountChangedEventPara>(_ =>
        {
            RefreshView();
        }).UnRegisterWhenDisabled(gameObject);
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

public class AddCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<CounterAppModel>().Count++;
        this.SendEvent<OnCountChangedEventPara>();
    }
}

public class MinusCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<CounterAppModel>().Count--;
        this.SendEvent<OnCountChangedEventPara>();
    }
}

public struct OnCountChangedEventPara
{
}





