using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    readonly UpdateTimer saveTimer = new (5f,()=>{
        MainModel.Save("Update");
    });
    [SerializeField]
    MainView prefabMainView;
    [SerializeField]
    BattleView prefabBattleView;
    void Awake()
    {
        MyEvent.ClearAll();
        MainView mainView = Instantiate(prefabMainView);
        mainView.Init();
        BattleView battleView = Instantiate(prefabBattleView);
        battleView.Init();
        MainModel.Init();
    }
    public void Update()
    {
        saveTimer.Tick(Time.deltaTime);
        MainModel.Tick(Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}