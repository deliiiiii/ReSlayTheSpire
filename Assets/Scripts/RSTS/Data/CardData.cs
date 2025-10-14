using System;
using RSTS.CDMV;

namespace RSTS;

[Serializable]
public class CardData(CardConfigMulti config)
{
    public int UpgradeLevel;
    public CardConfigMulti Config = config;
}