using System;

namespace RSTS;
[CardID(4)][Serializable]
public class Card4 : CardDataBase
{
    public override void OnCreate()
    {
        AddComponent<CardHasTarget>();
    }

    int attackValue => EmbedInt(0);

    public override void Yield(BothTurnData bothTurnData)
    {
        var target = GetComponent<CardHasTarget>().Target;
        if (target == null)
        {
            throw new ArgumentNullException($"No target for {nameof(Card4)}");
        }
        bothTurnData.AttackEnemy(target, attackValue);
        // (atk + strength) * (1 - weak) * (1 + vulnerable) * (1 + backAttack)
    }
}