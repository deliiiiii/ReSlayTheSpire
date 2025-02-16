using FairyGUI;

public class TestUI : Singleton<TestUI>
{
    public GComponent component1;
    GButton buttonRefine;
    protected override void OnInit()
    {
        GRoot.inst.SetContentScaleFactor(1920, 1080);
        component1 = GetComponent<UIPanel>().ui;
        buttonRefine = component1.GetChild("buttonRefine").asButton;
        buttonRefine.onClick.Add(() => OnClickRefine());
    }
    public void OnClickRefine()
    {
        Test.Instance.coin.Cost();
        Test.Instance.weapon.Refine();
    }
}