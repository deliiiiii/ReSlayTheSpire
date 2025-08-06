using System;
using UnityEngine;

namespace Violee;

[Serializable]
public class MapData
{
    public MyDictionary<Vector2Int, BoxData> BoxDataDic = [];
    public DateTime DateTime;
    
    public MapData()
    {
        Binder.Update(dt =>
        {
            if (GameManager.IsPlaying && !GameManager.HasPaused)
            {
                DateTime = DateTime.AddSeconds(dt * Configer.SettingsConfig.TimeSpeed);
            }
        });
    }
}