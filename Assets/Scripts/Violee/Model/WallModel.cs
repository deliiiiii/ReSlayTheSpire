using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

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
            if (Data.DoorType == EDoorType.None || Data.Opened.Value)
                return InteractInfo.CreateUnActive();
            // TODO BuffedValue...
            var energyCost = BuffManager.IsWithRecordPlayer ? 0 : 1;
            if (MainItemMono.EnergyCount.Value < energyCost)
            {
                return InteractInfo.CreateInvalid($"精力不足{energyCost}，无法打开门");
            }
            return new DoorInteractInfo
            {
                CanUse = true,
                Act = () =>
                {
                    MainItemMono.EnergyCount.Value -= energyCost;
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
                    DrawConfig defaultConfig = pool[0];
                    Enumerable.Range(0, 3).ForEach(i =>
                    {
                        if(pool.Count == 0)
                        {
                            ret.Add(defaultConfig);
                            return;
                        }
                        var added = pool.RandomItem();
                        pool.Remove(added);
                        ret.Add(added);
                    });
                    return ret;
                }
            };
        }
        
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
            switch (Data.DoorType)
            {
                case EDoorType.None:
                    NotDoor.SetActive(true);
                    WallSprite.sortingOrder = 25;
                    break;
                case EDoorType.Wooden:
                    Door.SetActive(true);
                    WallSprite.sortingOrder = 5;
                    break;
            }
        }
    }
}