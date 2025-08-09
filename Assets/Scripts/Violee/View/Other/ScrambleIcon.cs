using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

public class ScrambleIcon : MonoBehaviour
{
    public required Text LetterTxt;
    public required GameObject SelectedObj;
    public required Button Btn;
    public int InitID;


    void Awake()
    {
        InitID = transform.GetSiblingIndex();
        
        Binder.From(Btn).To(() => ScrambleMono.TryReverseSelected(this));
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