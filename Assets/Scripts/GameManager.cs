using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    MyFSM gameFSM;
    void Awake()
    {
        gameFSM = new(typeof(MyStateExamble));
    }
    void Update()
    {
        gameFSM.Update();
    }
}