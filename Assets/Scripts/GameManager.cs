using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static MyFSM gameFSM;
    void Awake()
    {
        gameFSM = new MyFSM(typeof(TitleState));
    }
    void Update()
    {
    }
}