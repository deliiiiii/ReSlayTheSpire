using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    
    
    [SerializeField]
    MainView prefabMainView;
    
    MainView mainViewIns;
    [SerializeField]
    BattleView prefabBattleView;
    BattleView battleViewIns;
    
    void Awake()
    {
        MyEvent.ClearAll();
        MainModel.Init();
        
        mainViewIns = Instantiate(prefabMainView);
        mainViewIns.Init();
        battleViewIns = Instantiate(prefabBattleView);
        battleViewIns.Init();
        
        MainModel.ChangeState(typeof(WaitForStartState_Title));
    }
    
    
    
    public void Update()
    {
        MainModel.Tick(Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}