using Sirenix.OdinInspector;
using UnityEngine;
using Violee;

public class MapModel : MonoBehaviour
{
    [Header("Map Settings")]
    public int Height = 4;
    public int Width = 6;
    public Vector2Int StartPos = Vector2Int.zero;
    public EBoxDir StartDir = EBoxDir.Up;
}
