using FairyGUI;

public class TestUI : Singleton<TestUI>
{
    public GComponent component1;

    public GTextField textAttack;
    public GTextField textAttackC;
    public GTextField textCoin;
    GTextField textCoinC;

    public GTextField textCurHP;
    public GTextField textMaxHP;
    public GTextField textDefend;
    public GTextField textDefendC;

    public GButton buttonHit;
    public GButton buttonRefine;
    

    protected override void OnInit()
    {
        GRoot.inst.SetContentScaleFactor(1920, 1080);
        component1 = GetComponent<UIPanel>().ui;

        textAttack = component1.GetChild("textAttack").asCom.GetChild("text0").asTextField;
        textAttackC = component1.GetChild("textAttackC").asCom.GetChild("text0").asTextField;
        textAttackC.text = "Attack";

        textCoin = component1.GetChild("textCoin").asCom.GetChild("text0").asTextField;
        textCoinC = component1.GetChild("textCoinC").asCom.GetChild("text0").asTextField;
        textCoinC.text = "Coin";

        textCurHP = component1.GetChild("textCurHP").asCom.GetChild("text0").asTextField;
        component1.GetChild("textHPSlash").asCom.GetChild("text0").text = "/";
        textMaxHP = component1.GetChild("textMaxHP").asCom.GetChild("text0").asTextField;
        textDefend = component1.GetChild("textDefend").asCom.GetChild("text0").asTextField;
        textDefendC = component1.GetChild("textDefendC").asCom.GetChild("text0").asTextField;
        textDefendC.text = "Defend";

        buttonHit = component1.GetChild("buttonHit").asButton;
        buttonRefine = component1.GetChild("buttonRefine").asButton;


    }

    EventCallback0 onClickRefine;
    EventCallback0 onClickHit;
    public void AddOnClickRefine(Store store)
    {
        onClickRefine = () => store.OnClickRefine();
        buttonRefine.onClick.Add(onClickRefine);
    }
    public void RemoveOnClickRefine()
    {
        buttonRefine.onClick.Remove(onClickRefine);
    }

    public void AddOnClickHit(WeaponClick weaponClick)
    {
        onClickHit = () => weaponClick.OnClickHit();
        buttonHit.onClick.Add(onClickHit);
    }
    public void RemoveOnClickHit()
    {
        buttonHit.onClick.Remove(onClickHit);
    }

    

    

}