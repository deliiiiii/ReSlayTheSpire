using System;
using UnityEngine;

namespace Violee;

[Serializable]
public class SceneMiniItemData : DataBase
{
    
}

[Serializable]
public class SceneMiniItemDataBook : SceneMiniItemData
{
    public int CreativityGain;
}

[Serializable]
public class SceneMiniItemDataFood : SceneMiniItemData
{
    public int StaminaGain;
}