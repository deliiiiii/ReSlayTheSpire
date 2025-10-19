using System;

namespace RSTS;
[CardID(1)][Serializable]
public class Card1 : CardDataBase
{
    public override void OnCreate()
    {
        AddComponent<CardHasTarget>();
    }

    public override void Yield(BothTurnData bothTurnData)
    {
        
    }
}