namespace RSTS;
public abstract class AttackResultBase;
public class AttackResultBaseDie(int dmg) : AttackResultBase
{
    public int OverflowDamage = dmg;
}