using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    public class WallModel : ModelBase<WallData>
    {
        #region Drag In
        [SerializeField] Transform Door;
        [SerializeField] Transform NotDoor;
        [SerializeField] SpriteRenderer WallSprite;
        [SerializeField] GameObject LockedSprite;
        [SerializeField] GameObject UnlockedSprite;
        #endregion
        protected override void ReadDataInternal()
        {
            Door.gameObject.SetActive(false);
            NotDoor.gameObject.SetActive(false);
            if (!data.HasWall)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            Binder.From(data.HasFoundWall).To(v => WallSprite.enabled = v).Immediate();
            Binder.From(data.HasFoundDoor).To(_ =>SetDoorSprite()).Immediate();
            switch (data.DoorType)
            {
                case EDoorType.None:
                    NotDoor.gameObject.SetActive(true);
                    break;
                case EDoorType.Wooden:
                    Door.gameObject.SetActive(true);
                    break;
            }

            SetDoorSprite();
        }

        // event
        public void FindWallAndDoor()
        {
            data.HasFoundWall.Value = true;
            if(data.HasDoor)
                data.HasFoundDoor.Value = true;
        }
        void SetDoorSprite()
        {
            LockedSprite.SetActive(false);
            UnlockedSprite.SetActive(false);
            if (!data.HasFoundDoor)
            {
                return;
            }
            if (data.Opened)
                UnlockedSprite.SetActive(true);
            else
                LockedSprite.SetActive(true);
        }
    }
}