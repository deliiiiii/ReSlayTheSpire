using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    List<Card> cards = new();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cards.Add(new Card(new CardData(){cardName = "Test", isUpper = false}));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cards.Add(new Card(new CardData(){cardName = "Test", isUpper = true}));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cards[0].MyDestroy();
            cards.RemoveAt(0);
        }
    }
}