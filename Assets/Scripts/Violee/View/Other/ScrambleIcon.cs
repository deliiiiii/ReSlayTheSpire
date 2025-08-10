using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class ScrambleIcon : MonoBehaviour
{
    public required Text LetterTxt;
    public required GameObject SelectedObj;
    public required Button Btn;
    public int ID;


    void Awake()
    {
        
        Binder.From(Btn).To(() => ScrambleView.TryReverseSelected(this));
    }

    public void OnSelected()
    {
        SelectedObj.SetActive(true);
    }
    
    public void OnNotSelected()
    {
        SelectedObj.SetActive(false);
    }
}