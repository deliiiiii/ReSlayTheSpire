using FairyGUI;
using Package1;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : Singleton<TestUI>
{
    GComponent component1;
    GTextField textCurHP;
    GTextField textMaxHP;
    public void Init()
    {
        GRoot.inst.SetContentScaleFactor(1920, 1080);
        // UIPackage.AddPackage("FGUI/Release/Package1");
        component1 = GetComponent<UIPanel>().ui;
        textCurHP = component1.GetChild("textCurHP").asCom.GetChild("text0").asTextField;
        textMaxHP = component1.GetChild("textMaxHP").asCom.GetChild("text0").asTextField;
    }
}