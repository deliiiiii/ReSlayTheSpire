using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Awake()
    {
        MyDebug.Log("GameManager Awake");
        Card card1 = new(new CardData(){cardName = "Test", isUpper = true});
        Card card2 = new(new CardData(){cardName = "Test", isUpper = false});



    }
}