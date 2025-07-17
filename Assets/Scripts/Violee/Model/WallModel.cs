using UnityEngine;

namespace Violee
{
    public class WallModel : MonoBehaviour
    {
        [SerializeField]
        Transform Door;

        [SerializeField] Transform NotDoor;
        
        public void SetIsDoor(bool isDoor)
        {
            Door.gameObject.SetActive(isDoor);
            NotDoor.gameObject.SetActive(!isDoor);
        }
    }
}