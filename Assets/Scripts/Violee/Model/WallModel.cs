using UnityEngine;

namespace Violee
{
    public class WallModel : ModelBase<WallData>
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

            if (Data.DoorType != EDoorType.None)
            {
                DoorInteract.InteractCb = GetCb;
            }
        }

        InteractCb? GetCb()
        {
            if (Data.Opened.Value)
                return null;
            if (PlayerManager.StaminaCount.Value <= 0)
            {
                return new InteractCb
                {
                    Cb = () => {},
                    Description = "体力不足，无法打开门",
                    Color = Color.red,
                };
            }
            return new InteractCb
            {
                Cb = () =>
                {
                    PlayerManager.StaminaCount.Value -= 1;
                    Data.Opened.Value = true;
                },
                Description = "打开门：消耗1点体力",
                Color = Color.blue,
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