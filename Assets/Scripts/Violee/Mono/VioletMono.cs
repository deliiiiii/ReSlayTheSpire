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

    public Vector2Int SavedBoxPos;
    public BoxData SavedBoxData;
    public BoxData LoadedBoxData;


    SerializableDictionary<Vector2Int, BoxData> dic 
        => MapManager.DijkstraStream.SelectResult().BoxDataDic;
    [Button]
    public void Test()
    {
        var curData = SceneItemModel.Data.CreateNew([D]);
        dic[CreateSceneItemPos].SceneDataMyList.MyAdd(curData);
    }
    
    [Button]
    public void Save()
    {
        SavedBoxData = dic[SavedBoxPos];
        Saver.Save("DataViolee", "SavedBoxData", SavedBoxData);
    }
    
    [Button]
    public void Load()
    {
        LoadedBoxData = Saver.Load<BoxData>("DataViolee", "SavedBoxData");
        // dic.Remove(LoadedBoxData.Pos2D);
        // dic.Add(LoadedBoxData.Pos2D, LoadedBoxData);
    }

    // [ShowInInspector] SerializableDictionary<EWallType, EBoxDir> testDic = new SerializableDictionary<EWallType, EBoxDir>()
    // {
    //     { EWallType.S1, EBoxDir.Up },
    //     { EWallType.S2, EBoxDir.Down },
    // };
    //
    // [ShowInInspector] Dictionary<EWallType, EBoxDir> testDic2;
    //
    // [Button]
    // public void Save2()
    // {
    //     Saver.Save("DataViolee", "SavedTestDic", testDic);
    // }
    //
    // [Button]
    // public void Load2()
    // {
    //     testDic = Saver.Load<SerializableDictionary<EWallType, EBoxDir>>("DataViolee", "SavedTestDic");
    //     testDic2 = Saver.Load<Dictionary<EWallType, EBoxDir>>("DataViolee", "SavedTestDic");
    // }


    // [Button]
    // public void Save()
    // {
    //     Saver.Save("DataViolee", "SavedData", CurData);
    // }
    //
    // [Button]
    // public void Load()
    // {
    //     SavedData = Saver.Load<SceneItemData>("DataViolee", "SavedData");
    //     MapManager.BoxDataByPos(Pos).SceneDataMyList.MyAdd(SavedData);
    // }
}