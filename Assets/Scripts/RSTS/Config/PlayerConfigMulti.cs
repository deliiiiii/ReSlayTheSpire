﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。
[CreateAssetMenu(menuName = "RSTS/Player",fileName = "Player", order = 2)][PublicAPI][Serializable]
public class PlayerConfigMulti : ConfigMulti<PlayerConfigMulti>
{
    protected override string PrefixName => "Player";

    public EPlayerJob Job;
    public Dictionary<CardConfigMulti, int> InitialCardDic = [];
    public int MaxHP;
}