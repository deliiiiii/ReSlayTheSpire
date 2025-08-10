using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class LetterIcon : MonoBehaviour
{
    public required Transform VioleTTransform;
    public required Text Letter;
    public Action? OnComplete;
    void OnEnable()
    {
        var rt = GetComponent<RectTransform>();
        rt.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        rt.localScale = Vector3.one;

        var targetPos = VioleTTransform.position;
        float downOffset = 50f;

        Sequence seq = DOTween.Sequence();
        
        seq.Append(DOTween.To(
            () => rt.sizeDelta,
            x => rt.sizeDelta = x,
            new Vector2(120, 120),
            0.3f
        ).SetEase(Ease.OutBack));
        
        seq.Append(DOTween.To(
            () => rt.position,
            x => rt.position = x,
            rt.position + Vector3.down * downOffset,
            0.4f
        ).SetEase(Ease.InOutSine));
        
        seq.Append(DOTween.To(
            () => rt.position,
            x => rt.position = x,
            targetPos,
            1f
        ).SetEase(Ease.OutQuad));
        
        seq.Append(rt.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            OnComplete?.Invoke();
            gameObject.SetActive(false);
        });
    }
}