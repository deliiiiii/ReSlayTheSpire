using FairyGUI;

public class TestUI : Singleton<TestUI>
{
    public GComponent component1;
    GButton buttonRefine;
    protected override void OnInit()
    {
        GRoot.inst.SetContentScaleFactor(1920, 1080);
        component1 = GetComponent<UIPanel>().ui;
        component1.GetChild("buttonRefine").onClick.Add(() => OnClickRefine());
        component1.GetChild("buttonHit").onClick.Add(() => isHit = true);
    }
    public void OnClickRefine()
    {
        Test.Instance.coin.Cost();
        Test.Instance.weapon.Refine();
    }
    bool isHit = false;
    public bool ClickHit()
    {
        bool ret = isHit;
        isHit = false;
        return ret;
    }
}