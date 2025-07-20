using UnityEngine;

namespace Violee
{
    public class WallModel : MonoBehaviour
    {
        public WallData WallData;
        #region Drag In
        [SerializeField] Transform Door;
        [SerializeField] Transform NotDoor;
        #endregion
        public void SetDoor(EDoorType doorType)
        {
            var isDoor = doorType == EDoorType.Wooden;
            Door.gameObject.SetActive(isDoor);
            NotDoor.gameObject.SetActive(!isDoor);
        }
    }
}