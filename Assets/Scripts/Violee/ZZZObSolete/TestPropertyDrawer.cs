using System;
using UnityEngine;

namespace Violee;

public class TestPropertyDrawer : MonoBehaviour
{
    public required TestClass TestClass;
}

[Serializable]
public class TestClass
{
    [RandomizeFloat(10, 20)]
    public float Value;
    public float Value2;
}

public class RandomizeFloatAttribute(float min, float max) : PropertyAttribute
{
    public float Min = min;
    public float Max = max;
}