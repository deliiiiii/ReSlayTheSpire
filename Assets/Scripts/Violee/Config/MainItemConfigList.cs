using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(MainItemConfigList), menuName = "Violee/" + nameof(MainItemConfigList))]
public class MainItemConfigList : ScriptableObject
{
    public List<MainItemConfig> MainItemConfigs = [];
}