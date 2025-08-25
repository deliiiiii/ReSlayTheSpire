using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee;

// public enum EDrawType
// {
//     Purple,
// }

[Serializable]
public class DrawConfig
{
    // public EDrawType DrawType;
    public string DrawTitle = string.Empty;
    // public string DrawDes => ToDrawModels.Count == 1 ? ToDrawModels[0].Data.GetInteractDes() : string.Empty;
    public string DrawDes => ToDrawModels[0].Data.GetInteractDes();
    public Sprite Sprite = null!;
    
    public List<SceneItemModel> ToDrawModels = [];
}