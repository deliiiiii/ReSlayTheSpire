using UnityEngine;

namespace Violee
{
    public class WallModel : MonoBehaviour
    {
        [SerializeField]
        WallData wallData;
        #region Drag In
        [SerializeField]
        Transform Door;
        [SerializeField] Transform NotDoor;
        #endregion
        public void SetIsDoor(EDoorType doorType)
        {
            var isDoor = doorType == EDoorType.Wooden;
            Door.gameObject.SetActive(isDoor);
            NotDoor.gameObject.SetActive(!isDoor);
        }
    }
}