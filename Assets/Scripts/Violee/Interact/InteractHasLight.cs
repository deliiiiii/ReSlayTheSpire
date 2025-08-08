using System;
using UnityEngine;

namespace Violee;

[RequireComponent(typeof(Light))]
public class InteractHasLight : MonoBehaviour
{
    public required Light LightIns;
    public Color Color;
    public float Intensity = 1f;
    public float Radius = 5f;

    void OnValidate()
    {
        if (LightIns == null)
            return;
        LightIns.range = Radius;
        LightIns.color = Color;
        LightIns.intensity = Intensity;
    }
    
}