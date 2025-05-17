using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResourceManager
{
    public const string UIPrefix = "Prefabs/UI/";
    public static async UniTask LoadRes()
    {
        await LoadUI();
    }

    static async UniTask LoadUI()
    {
        await Resourcer.LoadAssetsAsyncByLabel("UI");
        var allUILocations = await Resourcer.LoadResourceLocationsAsync("UI");
        foreach (var location in allUILocations)
        {
            var go = Resourcer.GetAssetFromCache<GameObject>(location.PrimaryKey);
            ViewBase.RegisterView(go.GetComponent<ViewBase>());
        }
    }
}
