using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class HurtEffect : MonoBehaviour
{
    public Text TxtHurt;
    public float MoveY = 40f;
    public float Duration = 0.9f;
    public Ease EaseType = Ease.OutCubic;
    public Canvas ParentCanvas;

    public bool Test;
    
    void OnEnable()
    {
        if(Test)
            return;
        transform.SetParent(ParentCanvas.transform);
        var rt = TxtHurt.rectTransform;
        rt.anchoredPosition += new Vector2(Random.Range(-50f, 50f), 0f);
        
        // var startColor = TxtHurt.color;
        // startColor.a = 1f;
        // TxtHurt.color = startColor;
        TxtHurt.transform.localScale = Vector3.one;

        DOTween.Kill(gameObject);

        var seq = DOTween.Sequence().SetTarget(gameObject);
        seq.Append(rt.DOAnchorPosY(rt.anchoredPosition.y + MoveY, Duration).SetEase(EaseType));
        seq.Join(TxtHurt.transform.DOScale(1.15f, Duration * 0.25f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo));
        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}