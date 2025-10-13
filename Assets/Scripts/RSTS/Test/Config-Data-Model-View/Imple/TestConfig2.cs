using System;
using UnityEngine;

namespace RSTS.Test;

[Serializable]
[CreateAssetMenu(fileName = nameof(TestConfig2), menuName = "RSTS/Test/" + nameof(TestConfig2), order = 0)]
public class TestConfig2 : ConfigBase
{
    public float Float;
}