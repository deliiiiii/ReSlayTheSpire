using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Violee
{
    public class WallModel : ModelBase<WallData>, IHasInteractReceiver
    {
        #region Inspector
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
        [SerializeField] GameObject Door;
        [SerializeField] GameObject NotDoor;
        [SerializeField] SpriteRenderer WallSprite;
        [SerializeField] GameObject LockedMesh;
        [SerializeField] GameObject UnLockedMesh;
        [SerializeField] GameObject LockedSprite;
        [SerializeField] GameObject UnlockedSprite;

        [SerializeField] InteractReceiver DoorInteract;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
        #endregion
        
        
        protected override void OnReadData()
        {
            SetAllActive();
            DoorInteract.GetInteractInfo = GetCb;
        }

        public InteractInfo GetCb()
        {
            if (!Data.HasDoor || Data.Opened.Value ||
                (PlayerMono.PlayerCurPoint.Value?.NextPointAndWallSet.All(pair => pair.Item2 != Data) ?? true))
                return InteractInfo.CreateUnActive();
            var energyCost = MainItemMono.CheckEnergyCost(1);
            if (MainItemMono.EnergyCount < energyCost)
            {
                return InteractInfo.CreateInvalid($"精力不足{energyCost}，无法打开门");
            }
            return new DoorInteractInfo
            {
                CanUse = true,
                Act = () =>
                {
                    MainItemMono.CostEnergy(energyCost);
                    Data.Opened.Value = true;
                },
                Description = $"打开门：消耗{energyCost}点精力, 并绘制门后连通区域的装饰。",
                Color = Color.magenta,
                
                InsidePointDataList = PlayerMono.PlayerCurPoint.Value?
                    .AtWallGetInsidePoints(Data).ToList() ?? [],
                WallData = Data,
                // GetDrawConfigs = () => Configer.DrawConfigList.DrawConfigs.Take(3).ToList(),
                GetDrawConfigs = () =>
                {
                    List<DrawConfig> ret = [];
                    List<DrawConfig> pool = [..Configer.DrawConfigList.DrawConfigs];
                    DrawConfig dreamCatcherConfig = pool.First(d => d.ToDrawModels.Any(m => m.Data.ID == 14));
                    
                    // float dreamRatio = MainItemMono.VioleTRequireCount / 1f / MapManager.DoorCount;
                    double dreamRatio = 1f / MapManager.DoorCount;
                    if (Configer.SettingsConfig.DreamCatcherGachaUp)
                        dreamRatio *= 3;
                        // // 1 - (1 - x)^3 = 1/16
                        // dreamRatio = (1 - Math.Pow(15.0 / 16.0, 1.0 / 3.0)) * 3;
                    
                    MyDebug.Log("Dream Ratio: " + dreamRatio);
                    Enumerable.Range(0, 3).ForEach(i =>
                    {
                        var added = Random.Range(0, 1f) <= dreamRatio ? dreamCatcherConfig : pool.RandomItem(x => x != dreamCatcherConfig && !lastestDrawnConfigs.Contains(x));
                        pool.Remove(added);
                        ret.Add(added);
                        lastestDrawnConfigs.Enqueue(added);
                        if (lastestDrawnConfigs.Count >= 15)
                            lastestDrawnConfigs.Clear();
                    });
                    return ret;
                }
            };
        }
        
        static readonly Queue<DrawConfig> lastestDrawnConfigs = [];
        public static void ClearLatestDrawn() => lastestDrawnConfigs.Clear();
        
        void SetAllActive()
        {
            Door.SetActive(false);
            NotDoor.SetActive(false);
            LockedMesh.SetActive(false);
            UnLockedMesh.SetActive(false);
            LockedSprite.SetActive(false);
            UnlockedSprite.SetActive(false);
            gameObject.SetActive(true);
            
            Binder.From(Data.Visited).To(v =>
            {
                WallSprite.enabled = v;
                if (!v)
                    return;
                if (Data.Opened)
                {
                    UnlockedSprite.SetActive(true);
                    LockedSprite.SetActive(false);
                }
                else
                {
                    LockedSprite.SetActive(true);
                }
            }).Immediate();
            Binder.From(Data.Opened).To(b =>
            {
                if (b)
                {
                    UnLockedMesh.SetActive(true);
                    LockedMesh.SetActive(false);
                }
                else
                {
                    LockedMesh.SetActive(true);
                }
                if (!Data.Visited)
                    return;
                if (b)
                {
                    UnlockedSprite.SetActive(true);
                    LockedSprite.SetActive(false);
                }
                else
                {
                    LockedSprite.SetActive(true);
                }
            }).Immediate();
            switch (Data.HasDoor)
            {
                case false:
                    NotDoor.SetActive(true);
                    WallSprite.sortingOrder = 25;
                    break;
                default:
                    Door.SetActive(true);
                    WallSprite.sortingOrder = 5;
                    break;
            }
        }
    }
}