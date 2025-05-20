using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int MaxHP;
    public int CurHP;
    public int Coin;
    public List<CardData> DeckCards;
    public List<BottleData> Bottles;

    public static PlayerData GetDefault()
    {
        return new PlayerData
        {
            MaxHP = 75,
            CurHP = 75,
            Coin = 99,
            DeckCards = new List<CardData>
            {
                new(1, false),
                new(1, true),
                new(1, true),
                new(2, false),
                new(2, false),
                new(2, true),
                new(2, true),
                new(3, false),
                new(4, false),
            },
            Bottles = new List<BottleData>()
        }; 
    }
}