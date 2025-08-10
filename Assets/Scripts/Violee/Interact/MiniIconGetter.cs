using UnityEngine;

namespace Violee;

public class MiniIconGetter : MonoBehaviour
{
    public required GameObject MiniIcon;

    public void SetEnable(bool enable)
    {
        MiniIcon.GetComponent<SpriteRenderer>().enabled = enable;
    }
}