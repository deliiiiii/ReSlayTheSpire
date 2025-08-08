using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class MusicWindow : MonoBehaviour
{
    public required Image BackImg;
    public required Text BGMTxt;
    public float DelayOnShown = 2f;
    
    RectTransform rect = null!;
    Vector2 tarPos;
    Sequence? seq;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        tarPos = rect.anchoredPosition;
    }

    void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        seq?.Kill();
        seq = DOTween.Sequence();
        var width = rect.rect.width;
        rect.anchoredPosition = new Vector2(-width, tarPos.y);

        seq.Append(rect.DOMoveX(0, 0.5f)
            .SetEase(Ease.OutBack));
        
        seq.AppendInterval(DelayOnShown);
        seq.AppendCallback(() => gameObject.SetActive(false));
    }
    
}