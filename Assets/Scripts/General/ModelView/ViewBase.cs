using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class ViewBase : MonoBehaviour
{
    /// <summary>
    /// Init, Bind, Launch
    /// </summary>
    protected abstract void IBL();

    void Awake()
    {
        IBL();
    }
}