using System;
using UnityEngine;

namespace Violee.View;

public class TransformSetter : MonoBehaviour
{
    public bool HeightListenToWidth;

    RectTransform rect = null!;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.sizeDelta = HeightListenToWidth 
            ? new Vector2(rect.sizeDelta.x, rect.sizeDelta.x) 
            : new Vector2(rect.sizeDelta.y, rect.sizeDelta.y);
    }
}