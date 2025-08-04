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
    public string Des = string.Empty;
    public Sprite Sprite = null!;
    
    public List<DrawAtPoint> DrawPoints = [];
}

[Serializable]
public class DrawAtPoint
{
    [SerializeReference]
    public SceneItemConfig? SceneItemConfig;
    public int Possibility;
}