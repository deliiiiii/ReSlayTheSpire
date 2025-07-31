using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(SceneItemConfigList), menuName = "Violee/" + nameof(SceneItemConfigList))]
public class SceneItemConfigList : ScriptableObject
{
    [SerializeReference]
    public List<SceneItemConfig> SceneItemConfigs = [];
}