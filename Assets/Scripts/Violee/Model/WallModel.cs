using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    public class WallModel : ModelBase<WallData>
    {
        #region Inspector
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
        [SerializeField] Transform Door;
        [SerializeField] Transform NotDoor;
        [SerializeField] SpriteRenderer WallSprite;
        [SerializeField] GameObject LockedSprite;
        [SerializeField] GameObject UnlockedSprite;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
        #endregion
        
        
        protected override void OnReadData()
        {
            Door.gameObject.SetActive(false);
            NotDoor.gameObject.SetActive(false);
            LockedSprite.SetActive(false);
            UnlockedSprite.SetActive(false);
            gameObject.SetActive(true);
            Binder.From(data.Visited).To(v =>
            {
                WallSprite.enabled = v;
                if(v)
                    SetDoorSprite();
            }).Immediate();
            switch (data.DoorType)
            {
                case EDoorType.None:
                    NotDoor.gameObject.SetActive(true);
                    break;
                case EDoorType.Wooden:
                    Door.gameObject.SetActive(true);
                    break;
            }
        }
        void SetDoorSprite()
        {
            if (data.Opened)
                UnlockedSprite.SetActive(true);
            else
                LockedSprite.SetActive(true);
        }
    }
}