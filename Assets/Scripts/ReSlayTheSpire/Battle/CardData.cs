using System;

public enum CardColor
{
    None,
    Red,
}
public enum CardType
{
    Attack,
    Skill,
    Ability,
    Curse,
}

[Serializable]
public class CardData
{
    private CardData(){}
    public int CardId;
    public bool IsUpper;
    public string CardName => "testCardName";
    public int CardCost => IsUpper ? 2 : 1;
    public CardColor CardColor => CardColor.Red;
    public CardType CardType => CardType.Attack;
    public CardData(int cardId, bool isUpper)
    {
        CardId = cardId;
        IsUpper = isUpper;
    }
    public CardData GetUpperCard()
    {
        return new CardData(CardId, true);
    }
}