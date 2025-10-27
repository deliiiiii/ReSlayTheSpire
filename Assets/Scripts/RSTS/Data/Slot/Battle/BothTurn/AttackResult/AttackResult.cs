using RSTS;

public abstract class AttackResult
{
    // public required HPAndBuffData Data;
}

public class AttackResultDie(int dmg) : AttackResult
{
    public int OverflowDamage = dmg;
}