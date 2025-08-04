using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee.View;

public class TransformSetter : MonoBehaviour
{
    public enum EAlignDir
    {
        Up,
        Down,
        Left,
        Right
    };
    
    public bool HeightAndWidthListenEach;
    [ShowIf(nameof(HeightAndWidthListenEach))]
    public bool HeightListenToWidth;

    public bool SizeListenToScreenMin;
    [ShowIf(nameof(SizeListenToScreenMin))]
    public float WidthDelta = 1;
    [ShowIf(nameof(SizeListenToScreenMin))]
    public float HeightDelta = 1;

    public bool ShouldAlign;
    [ShowIf(nameof(ShouldAlign))]
    public EAlignDir AlignDir;
    RectTransform rect = null!;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 设置gameObject的 RectTransform长宽
        // FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, Screen.height);
        if(HeightAndWidthListenEach)
            rect.sizeDelta = HeightListenToWidth 
                ? new Vector2(rect.sizeDelta.x, rect.sizeDelta.x) 
                : new Vector2(rect.sizeDelta.y, rect.sizeDelta.y);

        if (SizeListenToScreenMin)
        {
            var min = Mathf.Min(Screen.width * WidthDelta, Screen.height * HeightDelta);
            rect.sizeDelta = new Vector2(min, min);
        }

        if (ShouldAlign)
        {
            if(AlignDir == EAlignDir.Left)
                rect.anchoredPosition  = new Vector2(rect.sizeDelta.x / 2, 0);
            else if(AlignDir == EAlignDir.Right)
                rect.anchoredPosition  = new Vector3(-rect.sizeDelta.x / 2, 0, 0);
            else if(AlignDir == EAlignDir.Up)
                rect.anchoredPosition  = new Vector3(0, rect.sizeDelta.y / 2, 0);
            else if(AlignDir == EAlignDir.Down)
                rect.anchoredPosition  = new Vector3(0, -rect.sizeDelta.y / 2, 0);
        }
    }
}