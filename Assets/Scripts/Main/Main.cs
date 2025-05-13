using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField]
    MainView mainViewIns;
    [SerializeField]
    BattleView battleViewIns;
    
    void Awake()
    {
        MyEvent.ClearAll();
        MainModel.Init();
        mainViewIns.Init();
        battleViewIns.Init();
        
        mainViewIns.gameObject.SetActive(true);
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