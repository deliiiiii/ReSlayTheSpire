using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

namespace Violee;

public abstract class ViewBase<T> : Singleton<ViewBase<T>> where T : Singleton<T>
{
    [field: MaybeNull]protected T viewedModel => field ??= Singleton<T>.Instance;

    protected override void Awake()
    {
        base.Awake();
        Bind();
    }

    protected abstract void Bind();
}