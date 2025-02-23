using UnityEngine;
using QFramework;
using UnityEngine.UI;
using System.Collections.Generic;
namespace QFTest
{

    public class CounterAppArchitecture : Architecture<CounterAppArchitecture>
    {
        protected override void Init()
        {
            this.RegisterModel(new CounterAppModel());
            this.RegisterUtility(new QJsonIO());
        }
    }

    public struct CounterData
    {
        public int Count;
        public override readonly string ToString()
        {
            return $"Count: {Count}";
        }
    }
    public class CounterAppModel : AbstractModel
    {
        CounterData counterData;
        public int Count
        {
            get
            {
                return this.GetUtility<QJsonIO>().Load<CounterData>("Data","Count").Count;
            }
            set
            {
                if(counterData.Count != value)
                {
                    counterData.Count = value;
                    this.GetUtility<QJsonIO>().Save("Data","Count",counterData);
                }
            }
        }
        protected override void OnInit()
        {
            CounterData loadedData = this.GetUtility<QJsonIO>().Load<CounterData>("Data", "Count");
            if (loadedData.Equals(default))
            {
                Count = 0;
            }
            else
            {
                Count = loadedData.Count;
            }
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



        void Update()
        {
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


    public class QJsonIO : IUtility
    {
        public void Save<T>(string f_pathPre,string f_name,T curEntity)
        {
            JsonIO.Write(f_pathPre,f_name,curEntity);
        }
        public T Load<T>(string f_pathPre,string f_name)
        {
            return JsonIO.Read<T>(f_pathPre,f_name);
        }
        public void Delete(string f_pathPre,string f_name)
        {
            JsonIO.Delete(f_pathPre,f_name);
        }
    }
}
