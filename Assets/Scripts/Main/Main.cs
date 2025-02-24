using UnityEngine;
using QFramework;

public class Main : MonoBehaviour
{
    readonly UpdateTimer saveTimer = new (5f,()=>{
        MainModel.Save();
    });

    void Awake()
    {
        MainView.Instance.Init();
        MainModel.Init();
    }
    public void Update()
    {
        saveTimer.Tick(Time.deltaTime);
        MainModel.Tick(Time.deltaTime);
    }

}