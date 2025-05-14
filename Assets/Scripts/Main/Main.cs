using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    void Awake()
    {
        MainModel.Init();
        GlobalView.Instance.OnInit();
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