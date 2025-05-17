using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    void Awake()
    {
        UIManager.MainView.IBL();
    }
}