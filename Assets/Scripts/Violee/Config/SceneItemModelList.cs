using System.Collections.Generic;
using UnityEngine;

namespace Violee;

[CreateAssetMenu(fileName = nameof(SceneItemModelList), menuName = "Violee/" + nameof(SceneItemModelList))]
public class SceneItemModelList : ScriptableObject
{
    public List<SceneItemModel> SceneItemModels = [];
}