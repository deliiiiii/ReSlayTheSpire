namespace RSTS;

public abstract class AttackModifyBase;

public class AttackModifyCard4: AttackModifyBase
{
    public int StrengthMulti;
}

public class AttackModifyCard6: AttackModifyBase
{
    public int BaseAtkAddByDaJi;
}

public class AttackModifyCard12: AttackModifyBase
{
    public int AtkByBlock;
}

public class AttackModifyCard19: AttackModifyBase
{
    public int BaseAtkAddByUse;
}

public class AttackModifyCard28: AttackModifyBase
{
    public int AtkTimeByExhaust;
}

public class AttackModifyFromBuff : AttackModifyBase;