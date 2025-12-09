namespace RSTS;
public abstract class AttackResultBase;
public class AttackResultDie(int dmg) : AttackResultBase
{
    public readonly int OverflowDamage = dmg;
}

public class AttackResultHurt(int dmg) : AttackResultBase
{
    public int HurtDamage = dmg;
}