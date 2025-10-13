using System;
using UnityEngine;

namespace RSTS.Test;

public class TestModel : MonoBehaviour
{
    [SerializeReference]
    public required TestData TestData;
    public required TestConfig TestConfig;

    void Awake()
    {
        TestData = DataBase.Create<TestData>(TestConfig);
    }
}