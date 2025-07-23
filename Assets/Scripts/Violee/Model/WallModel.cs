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
            Door.gameObject.SetActive(false);
            NotDoor.gameObject.SetActive(false);
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
            WallData.HasFoundWall.Value = true;
            if(WallData.HasDoor)
                WallData.HasFoundDoor.Value = true;
        }
        void SetDoorSprite()
        {
            LockedSprite.SetActive(false);
            UnlockedSprite.SetActive(false);
            if (!WallData.HasFoundDoor)
            {
                return;
            }
            if (WallData.Opened)
                UnlockedSprite.SetActive(true);
            else
                LockedSprite.SetActive(true);
        }
    }
}