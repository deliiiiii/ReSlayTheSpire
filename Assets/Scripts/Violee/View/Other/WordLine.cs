using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class WordLine : MonoBehaviour
{
    public required Transform WordContent;
    public required List<ScrambleIcon> ScrambleIconList = [];
    public required Button ChooseWordBtn;
    public required Text ChooseWordTxt;
    string word = string.Empty;
    public Action<string>? OnWin;
    
    public void InitWithWord(string fWord)
    {
        for(int i = 0; i < WordContent.childCount; i++)
        {
            var wordObj = WordContent.GetChild(i).GetComponent<ScrambleIcon>();
            ScrambleIconList.Add(wordObj);
        }

        Binder.FromEvt(ChooseWordBtn.onClick).To(() =>
        {
            MyDebug.Log($"Win !! {word}");
            OnWin?.Invoke(word);
        });
        word = fWord;
        for (int i = 0; i < word.Length; i++)
        {
            var icon = ScrambleIconList[i];
            icon.gameObject.SetActive(true);
            icon.LetterTxt.text = fWord[i].ToString();
        }
        for (int i = word.Length; i < ScrambleIconList.Count; i++)
        {
            var icon = ScrambleIconList[i];
            icon.gameObject.SetActive(false);
        }
    }

    public bool RefreshGottenLetter(string gottenLetter)
    {
        int tarCount = word.Length;
        int curCount = 0;
        for (int i = 0; i < word.Length; i++)
        {
            if (gottenLetter.Contains(word[i]))
            {
                curCount++;
                // icon.SetGot();
            }
            else
            {
                // icon.SetNotGot();
            }
        }

        if (curCount < tarCount)
        {
            ChooseWordBtn.interactable = false;
            ChooseWordTxt.text = "未集齐";
            return false;
        }

        if (!gottenLetter.Contains(word))
        {
            ChooseWordBtn.interactable = false;
            ChooseWordTxt.text = "顺序不对..";
            return false;
        }
        ChooseWordBtn.interactable = true;
        ChooseWordTxt.text = "选择 →";
        return true;
    }
}