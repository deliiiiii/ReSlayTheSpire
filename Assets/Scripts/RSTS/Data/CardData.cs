using System;
using RSTS.CDMV;

namespace RSTS;

[Serializable]
public class CardData : DataBase<CardConfig>
{
    public int UpgradeLevel;
    protected override void OnReadConfig()
    {
        
    }
}