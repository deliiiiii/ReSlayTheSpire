using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee.View;

public class ScrambleMono : Singleton<ScrambleMono>
{
    public required Transform ScrambleIconParent;
    
    protected override void Awake()
    {
        base.Awake();
        SelectedList.OnAdd += icon =>
        {
            icon.OnSelected();
        };
        SelectedList.OnRemove += icon =>
        {
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
            for(int i = 0; i < vWindow!.Word.Length; i++)
            {
                var c = vWindow.Word[i];
                AllList[i].Btn.interactable = true;
                AllList[i].LetterTxt.text = c.ToString();
            }
            for(int i = vWindow.Word.Length; i < AllList.Count; i++)
            {
                AllList[i].LetterTxt.text = string.Empty;
                AllList[i].Btn.interactable = false;
            }
        };
    }

    [ShowInInspector]
    public static readonly MyList<ScrambleIcon> SelectedList = [];
    public static readonly List<ScrambleIcon> AllList = [];
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