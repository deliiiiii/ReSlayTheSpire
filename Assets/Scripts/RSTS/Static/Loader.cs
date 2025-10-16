using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS;
using RSTS.CDMV;

public static class Loader
{
    public static async Task LoadAll()
    {
        var configAll = new List<ConfigBase>(1000);
        configAll.AddRange(await Resourcer.LoadAssetsAsyncByLabel<ConfigBase>("RSTSConfig"));
        
        foreach (var config in configAll)
        {
            config.OnLoad();
        }
        
        CardDataBase.InitCardDic();
        EnemyDataBase.InitEnemyDic();
    }
}