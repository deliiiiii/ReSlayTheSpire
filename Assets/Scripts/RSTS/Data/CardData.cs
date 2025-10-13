using RSTS.CDMV;
using RSTS.Test;

namespace RSTS;

public class CardData : DataBase<CardConfig>
{
    public int UpgradeLevel;
    protected override void OnReadConfig()
    {
        
    }
}