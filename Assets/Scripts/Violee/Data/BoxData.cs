using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Violee
{
    public enum EBoxDir
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    public enum EWallType
    {
        S1 = 1 << 0,
        S2 = 1 << 1,
        S4 = 1 << 2,
        S8 = 1 << 3,
        T1248 = 1 << 4 | 1 << 6,
        T2481 = 1 << 5 | 1 << 7,
    }
    public static class BoxDataExt
    {
        public static bool HasSWallByDir(this BoxData self, EBoxDir dir, out WallData wallData)
        {
            if (self.WallDataMyDic.TryGetValue(BoxHelper.WallDirToType(dir), out var wallData2))
            {
                wallData = wallData2;
                return true;
            }
            wallData = null!;
            return false;
        }
        
        public static int CostStraight(this BoxData self, EBoxDir dir, out WallData? wallDataCanNull)
        {
            if (!HasSWallByDir(self, dir, out var wallData))
            {
                wallDataCanNull = null;
                return 0;
            }
            wallDataCanNull = wallData;
            if(wallData.DoorType == EDoorType.Wooden)
                return BoxData.DoorCost;
            return BoxData.WallCost;
        }
        
        public static int CostTilt(this BoxData self, EBoxDir from, EBoxDir to, out WallData? wallDataCanNull)
        {
            if (CanGoTiltWallBetween(self, from, to))
            {
                wallDataCanNull = null;
                return 0;
            }
            var t = (from, to) switch
            {
                (EBoxDir.Up, EBoxDir.Right)
                    or (EBoxDir.Right, EBoxDir.Up)
                    or (EBoxDir.Down, EBoxDir.Left)
                    or (EBoxDir.Left, EBoxDir.Down) => EWallType.T1248,
                (EBoxDir.Up, EBoxDir.Left)
                    or (EBoxDir.Left, EBoxDir.Up)
                    or (EBoxDir.Down, EBoxDir.Right)
                    or (EBoxDir.Right, EBoxDir.Down) => EWallType.T2481,
                _ => throw new ArgumentException($"from{from} and to{to} must be adjacent directions!"),
            };
            wallDataCanNull = self.WallDataMyDic[t];
            if(self.WallDataMyDic[t].DoorType == EDoorType.Wooden)
                return BoxData.DoorCost;
            return BoxData.WallCost;
        }

        /// <summary>
        /// dir1 dir2必须相邻！
        /// </summary>
        public static bool CanGoTiltWallBetween(this BoxData self, EBoxDir dir1, EBoxDir dir2)
        {
            var bigDir = dir1 > dir2 ? dir1 : dir2;
            var smallDif = dir1 < dir2 ? dir1 : dir2;
            var big = (byte)bigDir;
            var small = (byte)smallDif;
            // tilt walls
            byte x = (byte)(self.WallsByte >> 4);
            var fromDif = small;
            if (big == 8 && small == 1)
                fromDif = 8;
            return (big, small) switch
            {
                (4, 1) => (x & 3) != 3
                          && (x & 12) != 12
                          && (x & 5) != 5
                          && (x & 10) != 10,
                (8, 2) => (x & 9) != 9
                          && (x & 6) != 6
                          && (x & 5) != 5
                          && (x & 10) != 10,
                _ => x == 0 || x == fromDif || (x | fromDif) != x
            };
        }
    }
    [Serializable]
    public class BoxData : DataBase
    {
        [JsonConstructor]
        BoxData(int xxx = 0)
        {
            WallDataMyDic = [];
            WallDataMyDic.OnAdd += wallData => WallsByte |= (byte)wallData.WallType;
            WallDataMyDic.OnRemove += wallData => WallsByte |= (byte)wallData.WallType;
            PointDataMyDic = [];
            SceneDataMyList = [];
        }
        public BoxData(Vector2Int pos, BoxConfig config) : this()
        {
            Pos2D = pos;
            foreach (var dir in BoxHelper.AllBoxDirs)
            {
                PointDataMyDic.Add(dir, new BoxPointData()
                {
                    BelongBox = this,
                    Dir = dir,
                    Pos3D = BoxHelper.Pos2DTo3DPoint(Pos2D, dir),
                });
            }
            foreach (var dir in BoxHelper.AllBoxDirs)
            {
                foreach (var dir2 in BoxHelper.AllBoxDirs)
                {
                    if (dir == dir2)
                        continue;
                    if (BoxHelper.OppositeDirDic[dir] == dir2)
                        continue;
                    PointDataMyDic[dir].NextPointsInBox.Add(PointDataMyDic[dir2]);
                }
            }
            foreach (var wallType in BoxHelper.AllWallTypes)
            {
                WallsByte = config.Walls;
                if ((WallsByte & (int)wallType) == (int)wallType)
                    WallDataMyDic.Add(wallType, new WallData(wallType, EDoorType.Random){BelongBox = this});
            }
        }
        
        #region Field, Method...
        public Vector2Int Pos2D;
        public byte WallsByte;
        public const int WallCost = 10;
        public const int DoorCost = 1;
        public void ResetBeforeDij() 
            => PointDataMyDic.Values.ForEach(pointData => pointData.ResetBeforeDij());
        [JsonIgnore] public IEnumerable<EBoxDir> OccupiedDirs
            => SceneDataMyList.SelectMany(x => x.OccupyDirSet);
        #endregion
        
        
        #region List, Dic
        [ShowInInspector] public readonly MyDictionary<EWallType, WallData> WallDataMyDic;
        
        [ShowInInspector] public readonly Dictionary<EBoxDir, BoxPointData> PointDataMyDic;
        [ShowInInspector] public readonly MyList<SceneItemData> SceneDataMyList;

        #endregion
    }
}