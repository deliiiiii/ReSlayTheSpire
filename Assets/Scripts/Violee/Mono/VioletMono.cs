using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using UnityEngine;

namespace Violee;

public class VioletMono : MonoBehaviour
{
    [ShowInInspector] public string GameState => GameManager.GameState;
    [ShowInInspector] public List<WindowInfo> WindowList => GameManager.WindowList;
    void Awake()
    {
#if UNITY_EDITOR
        Binder.Update(_ => GUIHelper.RequestRepaint());
#endif
        GameManager.Init();
    }

    public Vector2Int CreateSceneItemPos;
    public EBoxDir D;
    public SceneItemModel SceneItemModel = null!;

    // public SceneItemData SavedData;
    // public SceneItemData CurData = null!;

    // public Vector2Int SavedBoxPos;
    // public BoxData SavedBoxData;
    // public BoxData LoadedBoxData;


    MyDictionary<Vector2Int, BoxData> dic 
        => MapManager.DijkstraStream.SelectResult().BoxDataDic;
    [Button]
    public void Test()
    {
        var curData = SceneItemModel.Data.CreateNew([D]);
        dic[CreateSceneItemPos].SceneDataMyList.MyAdd(curData);
    }
    
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