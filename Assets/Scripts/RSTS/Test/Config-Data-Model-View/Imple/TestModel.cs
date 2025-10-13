using System;
using UnityEngine;

namespace RSTS.Test;

public class TestModel : ModelBase<TestData>
{
    // 临时引用一个Config
    public required TestConfig TestConfig;

    void Awake()
    {
        Data = CreateData(TestConfig);
    }
}