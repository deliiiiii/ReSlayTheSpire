using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour, IController
{
    [SerializeField]
    Button btnStart;
    [SerializeField]
    Button btnQuit;


    [SerializeField]
    Text txtPlayTime;
    void Awake()
    {
        btnStart.onClick.AddListener(()=>{});
    }
    void Update()
    {
        this.GetSystem<GlobalSystem>().Update();
        // 保留1位小数
        txtPlayTime.text = this.GetModel<GlobalModel>().PlayTime.ToString("F1");
    }

    public IArchitecture GetArchitecture()
    {
        return GlobalArthitecture.Interface;
    }
}
