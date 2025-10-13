using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.Utilities;

namespace RSTS.Test;


[Serializable]
public class TestData : DataBase<TestConfig>
{
    public List<int> SpreadIdList = [];

    protected override void OnReadConfig()
    {
        for (var i = 0; i < Config.SpreadCount; i++)
        {
            SpreadIdList.Add(new Random().Next(0, Config.SpreadMaxId));
        }
    }
}