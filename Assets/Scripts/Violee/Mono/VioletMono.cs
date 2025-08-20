using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using UnityEngine;
using Violee.View;

namespace Violee;

public class VioletMono : Singleton<VioletMono>
{
    // [ShowInInspector] public string GameState => GameManager.GameState;
    // [ShowInInspector] public List<WindowInfo> WindowList => WindowManager.WindowList;

    void Start()
    {
#if UNITY_EDITOR
        Binder.Update(_ => GUIHelper.RequestRepaint());
#endif
        _ = InitAll();
    }

    static async Task InitAll()
    {
        Configer.Init();
        GameView.Init();
        MapView.Init();
        MainItemMono.Init();
        await AudioMono.Init();

        Mediator.Mediate();
        
        GameManager.EnterTitle();
    }
    // public Vector2Int CreateSceneItemPos;
    // public EBoxDir D;
    // public SceneItemModel SceneItemModel = null!;
    // MyDictionary<Vector2Int, BoxData> dic 
    //     => MapManager.DijkstraStream.SelectResult().BoxDataDic;
    // [Button]
    // public void Test()
    // {
    //     var curData = SceneItemModel.Data.CreateNew([D]);
    //     dic[CreateSceneItemPos].SceneDataMyList.MyAdd(curData);
    // }
    
    
    
    
    // public SceneItemData SavedData;
    // public SceneItemData CurData = null!;
    // public Vector2Int SavedBoxPos;
    // public BoxData SavedBoxData;
    // public BoxData LoadedBoxData;
    // [Button]
    // public void Save()
    // {
    //     SavedBoxData = dic[SavedBoxPos];
    //     Saver.Save("DataViolee", "SavedBoxData", SavedBoxData);
    // }
    //
    // [Button]
    // public void Load()
    // {
    //     LoadedBoxData = Saver.Load<BoxData>("DataViolee", "SavedBoxData");
    //     dic.Remove(LoadedBoxData.Pos2D);
    //     dic.Add(LoadedBoxData.Pos2D, LoadedBoxData);
    // }
}