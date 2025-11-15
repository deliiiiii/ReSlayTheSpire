using System.Collections.Generic;
using System.Threading.Tasks;
using RSTS.CDMV;

namespace RSTS;
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
        
        CardDataBase.InitCtorDic();
        // TODO Enemy跟Card一样管理好了。尝试一下基类带三个泛型参数有没有问题！
        EnemyDataBase.InitEnemyDic();
    }
}