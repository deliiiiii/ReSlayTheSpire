using System;
using UnityEngine;

namespace Violee;

public abstract class ViewBase<T> : Singleton<ViewBase<T>> where T : Singleton<T>
{
    protected T viewedModel;

    protected override void Awake()
    {
        base.Awake();
        viewedModel = Singleton<T>.Instance;
        Bind();
    }

    protected abstract void Bind();
}