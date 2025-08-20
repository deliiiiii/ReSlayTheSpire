using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(BoxConfigList), menuName = "Violee/" + nameof(BoxConfigList))]
public class BoxConfigList : ScriptableObject
{
    [Header("Map Settings")]
    [SerializeField] int height = 4;
    public int Height => Configer.SettingsConfig.UseSmallMap ? 5 : height;
    [SerializeField] int width = 6;
    public int Width => Configer.SettingsConfig.UseSmallMap ? 5 : width;
    [SerializeField] Vector2Int startPos = Vector2Int.zero;
    public Vector2Int StartPos => Configer.SettingsConfig.UseSmallMap ? new Vector2Int(2, 2) : startPos;
    public EBoxDir StartDir = EBoxDir.Up;
        
    [Header("Other")]
    [Range(0, 1)]
    public float DoorPossibility;
    [Range(0.4f, 0.5f)]
    public float WalkInTolerance = 0.45f;
    [SerializeField] List<BoxConfig> boxConfigs = [];

    public List<BoxConfig> BoxConfigs
    {
        get
        {
            if (!Configer.SettingsConfig.AddTiltWall)
            {
                boxConfigs
                    .Where(b => BoxHelper.HasTiltWallByByte(b.Walls))
                    .ForEach(b => b.BasicWeight = 0);
                return boxConfigs;
            }
            
            boxConfigs
                .Where(b => BoxHelper.HasTiltWallByByte(b.Walls))
                .ForEach(b => b.BasicWeight = 335);
            return boxConfigs;
        }
    }
}