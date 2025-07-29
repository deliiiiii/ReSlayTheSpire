using System;
using UnityEngine;

namespace Violee.Violee.Test;

public class Test : MonoBehaviour
{
}

public delegate void BuildRenderOperation(
    Action<string> setTip,
    Action<bool> setBuildable,
    Action<Vector2Int> onMove,
    Action<Vector2Int> onConfirm);