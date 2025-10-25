public abstract class AttackResult;

public class AttackResultDie(int dmg) : AttackResult
{
    public int OverflowDamage = dmg;
}