using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    void Awake()
    {
        MainModel.Init();
        UIManager.MainView.Bind();
        MainModel.ChangeState(EMainState.Title);
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