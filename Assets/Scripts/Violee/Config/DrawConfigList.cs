using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(DrawConfigList), menuName = "Violee/" + nameof(DrawConfigList))]
public class DrawConfigList : ScriptableObject
{
    public List<DrawConfig> DrawConfigs = [];
}