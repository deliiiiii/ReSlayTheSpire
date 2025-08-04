using System;
using System.Collections.Generic;
using UnityEngine;

namespace Violee;

public enum EDrawType
{
    Purple,
}

[Serializable]
public class DrawConfig
{
    public EDrawType DrawType;
    public string DrawDes = string.Empty;
    public Sprite Sprite = null!;
    
    public List<DrawAtPoint> DrawPoints = [];
}

[Serializable]
public class DrawAtPoint
{
    [SerializeReference]
    public SceneItemModel? SceneItemModel;
    public int Possibility;
}