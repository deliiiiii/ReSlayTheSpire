using System;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    MainUI mainUI;
    MainLogic mainLogic;
    void Awake()
    {
        mainUI = Instantiate(Resources.Load<MainUI>("MainUI"));
        mainLogic = new MainLogic(mainUI);
    }
    void Update()
    {
        mainLogic.Update();
    }
}
