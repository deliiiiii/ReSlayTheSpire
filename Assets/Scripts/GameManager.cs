using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static MyFSM gameFSM;
    void Awake()
    {
        gameFSM = new(typeof(MyStateExamble));
    }
    void Update()
    {
        gameFSM.Update();
    }
}