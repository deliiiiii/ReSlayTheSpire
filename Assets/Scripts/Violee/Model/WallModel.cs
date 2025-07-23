using UnityEngine;

namespace Violee
{
    public class WallModel : MonoBehaviour
    {
        public WallData WallData;
        #region Drag In
        [SerializeField] Transform Door;
        [SerializeField] Transform NotDoor;
        [SerializeField] SpriteRenderer WallSprite;
        [SerializeField] GameObject LockedSprite;
        [SerializeField] GameObject UnlockedSprite;
        #endregion
        public void ReadData(WallData wallData)
        {
            this.WallData = wallData;
            if (!WallData.HasWall)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            Binder.From(WallData.HasFoundWall).To(v => WallSprite.enabled = v).Immediate();
            Binder.From(WallData.HasFoundDoor).To(_ =>SetDoorSprite()).Immediate();
            switch (WallData.DoorType)
            {
                case EDoorType.None:
                    Door.gameObject.SetActive(false);
                    NotDoor.gameObject.SetActive(true);
                    break;
                case EDoorType.Wooden:
                    Door.gameObject.SetActive(true);
                    NotDoor.gameObject.SetActive(false);
                    break;
            }

            SetDoorSprite();
        }

        // event
        public void FindWallAndDoor()
        {
            WallData.HasFoundWall.Value = true;
            if(WallData.HasDoor)
                WallData.HasFoundDoor.Value = true;
        }
        void SetDoorSprite()
        {
            if (!WallData.HasFoundDoor)
            {
                LockedSprite.SetActive(false);
                UnlockedSprite.SetActive(false);
                return;
            }
            if (WallData.Opened)
            {
                LockedSprite.SetActive(false);
                UnlockedSprite.SetActive(true);
            }
            else
            {
                LockedSprite.SetActive(true);
                UnlockedSprite.SetActive(false);
            }
        }
    }
}