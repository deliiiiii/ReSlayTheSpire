using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class ScrambleView : Singleton<ScrambleView>
{
    public required Transform ScrambleIconParent;
    // public required Button ExchangeButton;
    public required Text ExchangeTxt;
    public float ExchangeDuration = 1f;
    public required GameObject ExchangingPnl;

    [ShowInInspector]
    public static readonly MyList<ScrambleIcon> SelectedList = [];
    public static readonly List<ScrambleIcon> AllList = [];
    
    public static readonly ExchangeWindowInfo ExchangeWindow = new()
    {
        Des = "交换字母中",
    };
    
    protected override void Awake()
    {
        var layoutGroup = ScrambleIconParent.GetComponent<HorizontalLayoutGroup>();
        var contentSizeFitter = ScrambleIconParent.GetComponent<ContentSizeFitter>();
        base.Awake();
        ExchangeWindow.OnAddEventWithArg += w =>
        {
            ExchangingPnl.SetActive(true);
            var seq = DOTween.Sequence();
            var first = SelectedList[0];
            var second = SelectedList[1];
            layoutGroup.enabled = false;
            contentSizeFitter.enabled = false;
            first.transform
                .DOMoveX(second.transform.position.x, ExchangeDuration)
                .SetEase(Ease.OutCirc);
            second.transform
                .DOMoveX(first.transform.position.x, ExchangeDuration)
                .SetEase(Ease.OutCirc);
            seq.AppendInterval(ExchangeDuration);
            seq.AppendCallback(() =>
            {
                SelectedList.MyClear();
                MainItemMono.ExchangeLetter(first.ID, second.ID);
                var tempID = first.ID;
                first.ID = second.ID;
                first.transform.SetSiblingIndex(first.ID);
                second.ID = tempID;
                second.transform.SetSiblingIndex(second.ID);
                layoutGroup.enabled = true;
                contentSizeFitter.enabled = true;
                GameManager.WindowList.MyRemove(ExchangeWindow);
                (w as ExchangeWindowInfo)!.OnExchangeEnd?.Invoke();
            });
            
        };
        ExchangeWindow.OnRemoveEvent += () =>
        {
            ExchangingPnl.SetActive(false);
        };
        
        SelectedList.OnAdd += icon =>
        {
            if (SelectedList.Count == 2)
            {
                EnableTxt();
            }
            icon.OnSelected();
        };
        SelectedList.OnRemove += icon =>
        {
            DisableBtn();
            icon.OnNotSelected();
        };
        for(int i = 0; i < ScrambleIconParent.childCount; i++)
        {
            var icon = ScrambleIconParent.GetChild(i).GetComponent<ScrambleIcon>();
            AllList.Add(icon);
        }
        GameView.VioleTWindow.OnAddEventWithArg += w =>
        {
            var vWindow = w as VioleTWindowInfo;
            SelectedList.MyClear();
            DisableBtn();
            AllList.Clear();
            for(int i = 0; i < ScrambleIconParent.childCount; i++)
            {
                var icon = ScrambleIconParent.GetChild(i).GetComponent<ScrambleIcon>();
                icon.ID = i;
                AllList.Add(icon);
            }

            var word = vWindow!.GetWord();
            for(int i = 0; i < word.Length; i++)
            {
                AllList[i].Btn.interactable = true;
                AllList[i].LetterTxt.text = word[i].ToString();
            }
            for(int i = word.Length; i < AllList.Count; i++)
            {
                AllList[i].Btn.interactable = false;
                AllList[i].LetterTxt.text = string.Empty;
            }
        };
    }

    void EnableTxt()
    {
        // ExchangeButton.interactable = true;
        ExchangeTxt.text = "交换中";
        GameManager.WindowList.MyAdd(ExchangeWindow);
    }

    void DisableBtn()
    {
        // ExchangeButton.interactable = false;
        ExchangeTxt.text = "↑ 选择两个字母牌交换它们 ↑";
    }
    public static void TryReverseSelected(ScrambleIcon icon)
    {
        if (SelectedList.Contains(icon))
        {
            SelectedList.MyRemove(icon);
            return;
        }

        if (SelectedList.Count >= 2)
            return;
        SelectedList.MyAdd(icon);
    }
}