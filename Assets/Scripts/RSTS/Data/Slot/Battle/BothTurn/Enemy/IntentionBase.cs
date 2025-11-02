using System;

namespace RSTS;

[Serializable]
public abstract class IntentionBase;

[Serializable]
public class IntentionAttack : IntentionBase
{
    public int Attack;
    public int AttackTime = 1;
}

[Serializable]
public class IntentionBlock : IntentionBase
{
    public int Block;
}