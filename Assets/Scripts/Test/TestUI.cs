using FairyGUI;
using Package1;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemy : Singleton<UIEnemy>
{
    GComponent component1;
    GTextField textCurHP;
    GTextField textMaxHP;
    protected override void OnInit()
    {
        GRoot.inst.SetContentScaleFactor(1920, 1080);
        // UIPackage.AddPackage("FGUI/Release/Package1");
        component1 = GetComponent<UIPanel>().ui;
        textCurHP = component1.GetChild("textCurHP").asCom.GetChild("text0").asTextField;
        component1.GetChild("textHPSlash").asCom.GetChild("text0").text = "/";
        textMaxHP = component1.GetChild("textMaxHP").asCom.GetChild("text0").asTextField;
    }

    public void Refresh(float curHP, float maxHP)
    {
        textCurHP.text = curHP.ToString();
        textMaxHP.text = maxHP.ToString();
    }
}