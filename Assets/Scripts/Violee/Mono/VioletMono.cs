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

    public Vector2Int Pos;
    public EBoxDir D;
    public SceneItemModel SceneItemModel = null!;

    // public SceneItemData SavedData;
    // public SceneItemData CurData = null!;

    public Vector2Int SavedBoxPos;
    public BoxData SavedBoxData;
    public BoxData LoadedBoxData;
    
    [Button]
    public void Test()
    {
        var CurData = SceneItemModel.Data.CreateNew([D]);
        MapManager.BoxDataByPos(Pos).SceneDataMyList.MyAdd(CurData);
    }
    
    [Button]
    public void Save()
    {
        SavedBoxData = MapManager.BoxDataByPos(SavedBoxPos);
        Saver.Save("DataViolee", "SavedBoxData", SavedBoxData);
    }
    
    [Button]
    public void Load()
    {
        LoadedBoxData = Saver.Load<BoxData>("DataViolee", "SavedBoxData");
        MapManager.AddTest(LoadedBoxData);
    }
    

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