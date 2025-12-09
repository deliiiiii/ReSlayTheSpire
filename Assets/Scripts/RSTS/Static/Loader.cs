using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RSTS.CDMV;
using RSTS;

namespace RSTS;
public static class Loader
{
    public static async UniTask LoadAll()
    {
        var configAll = new List<ConfigBase>(1000);
        configAll.AddRange(await Resourcer.LoadAssetsAsyncByLabel<ConfigBase>("RSTSConfig"));
        
        foreach (var config in configAll)
        {
            config.OnLoad();
        }
        
        CardInTurn.InitCtorDic();
        // TODO Enemy跟Card一样管理好了。尝试一下基类带三个泛型参数有没有问题！
        EnemyDataBase.InitEnemyDic();
    }
}