using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Violee
{
    public enum EBoxDir : byte
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    public enum EWallType : byte
    {
        S1 = 1 << 0,
        S2 = 1 << 1,
        S4 = 1 << 2,
        S8 = 1 << 3,
        T1248 = 1 << 4 | 1 << 6,
        T2481 = 1 << 5 | 1 << 7,
    }

    
    public class BoxPointData
    {
        public BoxPointData(BoxData belongBox)
        {
            BelongBox = belongBox;
        }
        
        public EBoxDir Dir;
        public Observable<int> CostWall;
        public Observable<int> CostStep;
        [NonSerialized]
        public List<BoxPointData> NextPointsInBox;
        [NonSerialized]
        public readonly BoxData BelongBox;
        [NonSerialized]
        public Vector2 Pos2D;
        public void UpdateNextPointCost()
        {
            foreach (var nextPoint in NextPointsInBox)
            {
                nextPoint.CostWall.Value = Math.Min(
                    nextPoint.CostWall.Value,
                    CostWall + BelongBox.CostTilt(Dir, nextPoint.Dir));
            }
        }
    }
    
    [Serializable]
    public class BoxData
    {
        BoxData(){}
        public static BoxData Create(Vector2Int pos, BoxConfigSingle config)
        {
            var ret = new BoxData()
            {
                Pos = pos,
                wallsByte = config.Walls,
            };
            foreach (var wallType in BoxHelper.AllWallTypes)
            {
                if((ret.wallsByte & (int)wallType) == (int)wallType)
                    ret.wallKList.Add(WallData.Create(wallType, EDoorType.Random));
            }
            return ret;
        }
        
        public Vector2Int Pos;
        // // TODO 1：之后在sprite上自己划线？ 2：拿预制体的3d模型
        // public Sprite Sprite;
        public event Action<WallData> OnAddWall;
        public event Action<WallData> OnRemoveWall;
        #region Walls
        [NonSerialized] 
        byte wallsByte;

        [NotNull] MyKeyedCollection<EWallType, WallData> wallKList = new(w => w.WallType);
        
        public static bool HasSWallByByteAndDir(byte walls, EBoxDir dir) => (walls & (byte)dir) != 0;

        public bool HasWallByType(EWallType wallType, out WallData wallData) => 
            wallKList.TryGetValue(wallType, out wallData);
        public bool HasSWallByDir(EBoxDir dir, out WallData wallData) => 
            wallKList.TryGetValue(BoxHelper.WallDirToType(dir), out wallData);
        public void AddSWall(WallData wallData)
        {
            RemoveSWall(wallData);
            wallKList.Add(wallData);
            wallsByte |= (byte)wallData.WallType;
            OnAddWall?.Invoke(wallData);
        }
        public void RemoveSWall(WallData newData)
        {
            if (wallKList.Contains(newData))
            {
                wallKList.Remove(newData);
                wallsByte &= (byte)~(byte)newData.WallType;
                OnRemoveWall?.Invoke(newData);
            }
        }
        #endregion
        
        
        #region Path
        public const int WallCost = 10;
        public const int DoorCost = 1;
        public MyKeyedCollection<EBoxDir, BoxPointData> PointKList;
        static float offset => Configer.SettingsConfig.BoxCostPosOffset;
        public void InitPoint()
        {
            PointKList = new MyKeyedCollection<EBoxDir, BoxPointData>(b => b.Dir);
            foreach (var dir in BoxHelper.AllBoxDirs)
            {
                PointKList.Add(new BoxPointData(this)
                {
                    Dir = dir,
                    CostWall = new (int.MaxValue / 2),
                    Pos2D = new (Pos.x + BoxHelper.DirToVec2Dic[dir].x * offset, 
                        Pos.y + BoxHelper.DirToVec2Dic[dir].y * offset),
                    NextPointsInBox = new List<BoxPointData>()
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
                    PointKList[dir].NextPointsInBox.Add(PointKList[dir2]);
                }
            }
        }

        public int CostStraight(EBoxDir dir)
        {
            if (!HasSWallByDir(dir, out var wallData))
                return 0;
            if(wallData.DoorType == EDoorType.Wooden)
                return DoorCost;
            return WallCost;
        }

        public int CostTilt(EBoxDir from, EBoxDir to)
        {
            if (CanGoTiltWallBetween(from, to))
                return 0;
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
            if(wallKList[t].DoorType == EDoorType.Wooden)
                return DoorCost;
            return WallCost;
        }
        
        /// <summary>
        /// dir1 dir2必须相邻！
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        bool CanGoTiltWallBetween(EBoxDir dir1, EBoxDir dir2)
        {
            var bigDir = dir1 > dir2 ? dir1 : dir2;
            var smallDif = dir1 < dir2 ? dir1 : dir2;
            var big = (byte)bigDir;
            var small = (byte)smallDif;
            // tilt walls
            var x = wallsByte >> 4;
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
        #endregion
    }
}