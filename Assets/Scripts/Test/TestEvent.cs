using FairyGUI;

#region 点击事件
public class OnClickRefine : MyEvent
{
    Weapon weapon;
    Coin coin;
    EventCallback0 onClick;
    public OnClickRefine(Weapon weapon, Coin coin)
    {
        this.weapon = weapon;
        this.coin = coin;
        onClick = () => Fire();
        UI.TestUI.buttonRefine.onClick.Add(onClick);
    }
    ~OnClickRefine()
    {
        UI.TestUI.buttonRefine.onClick.Remove(onClick);
    }
    public override void Fire()
    {
        coin.Cost();
        weapon.Refine();
    }
}

public class OnClickHit : MyEvent
{
    WeaponClick weaponClick;
    EventCallback0 onClick;
    public OnClickHit(WeaponClick weaponClick)
    {
        this.weaponClick = weaponClick;
        onClick = () => Fire();
        UI.TestUI.buttonHit.onClick.Add(onClick);
    }
    ~OnClickHit()
    {
        UI.TestUI.buttonHit.onClick.Remove(onClick);
    }
    public override void Fire()
    {
        weaponClick.clicked = true;
    }
}
#endregion

#region 数据变化
public class OnWeaponChange : MyEvent
{
    Weapon weapon;
    public OnWeaponChange(Weapon weapon)
    {
        this.weapon = weapon;
    }
    public override void Fire()
    {
        UI.TestUI.textAttack.text = weapon.Damage.ToString();
    }
}

public class OnMoneyChange : MyEvent
{
    Coin coin;
    public OnMoneyChange(Coin coin)
    {
        this.coin = coin;
    }
    public override void Fire()
    {
        UI.TestUI.textCoin.text = coin.Money.ToString();
        UI.TestUI.buttonRefine.enabled = coin.Money >= coin.Cost1;
        UI.TestUI.buttonRefine.text = $"Refine {coin.Cost1}";
    }
}

public class OnEnemyChange : MyEvent
{
    Enemy enemy;
    public OnEnemyChange(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public override void Fire()
    {
        UI.TestUI.textCurHP.text = enemy.Health.ToString();
        UI.TestUI.textMaxHP.text = enemy.MaxHP.ToString();
        UI.TestUI.textDefend.text = enemy.Defend.ToString();
        UI.TestUI.textDefend.visible = enemy.Defend > 0;
        UI.TestUI.textDefendC.visible = enemy.Defend > 0;
    }
}
#endregion
