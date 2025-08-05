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
    [Button]
    public void Test()
    {
        MapManager.BoxDataByPos(Pos).SceneDataMyList.Add(SceneItemModel.Data.CreateNew([D]));
    }
}