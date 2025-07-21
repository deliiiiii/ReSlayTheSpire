using UnityEngine;

namespace Violee
{
    public class WallModel : MonoBehaviour
    {
        public WallData WallData;
        #region Drag In
        [SerializeField] Transform Door;
        [SerializeField] Transform NotDoor;
        [SerializeField] SpriteRenderer LockedSprite;
        [SerializeField] SpriteRenderer UnlockedSprite;
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
            switch (wallData.DoorType)
            {
                case EDoorType.None:
                    Door.gameObject.SetActive(false);
                    NotDoor.gameObject.SetActive(true);
                    LockedSprite.enabled = false;
                    UnlockedSprite.enabled = false;
                    break;
                case EDoorType.Wooden:
                    Door.gameObject.SetActive(true);
                    NotDoor.gameObject.SetActive(false);
                    if (WallData.Opened)
                    {
                        LockedSprite.enabled = false;
                        UnlockedSprite.enabled = true;
                    }
                    else
                    {
                        LockedSprite.enabled = true;
                        UnlockedSprite.enabled = false;
                    }
                    break;
                default: break;
            }
            
        }
    }
}