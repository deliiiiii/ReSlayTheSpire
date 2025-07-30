using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(MiniItemConfigList), menuName = "Violee/" + nameof(MiniItemConfigList))]
public class MiniItemConfigList : ScriptableObject
{
    public List<MiniItemConfig> MiniItemConfigs = [];
}