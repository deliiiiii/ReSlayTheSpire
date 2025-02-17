#region 数据变化
public class OnWeaponChange : IEvent
{
    Weapon weapon;
    public OnWeaponChange(Weapon weapon)
    {
        this.weapon = weapon;
    }
    public void Fire()
    {
        UI.TestUI.textAttack.text = weapon.Damage.ToString();
    }
}

public class OnMoneyChange : IEvent
{
    CoinBag coinBag;
    public OnMoneyChange(CoinBag coinBag)
    {
        this.coinBag = coinBag;
    }
    public void Fire()
    {
        UI.TestUI.textCoin.text = coinBag.Coin.ToString();
    }
}

public class OnEnemyChange : IEvent
{
    Enemy enemy;
    public OnEnemyChange(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Fire()
    {
        UI.TestUI.textCurHP.text = enemy.Health.ToString();
        UI.TestUI.textMaxHP.text = enemy.MaxHP.ToString();
        UI.TestUI.textDefend.text = enemy.Defend.ToString();
        UI.TestUI.textDefend.visible = enemy.Defend > 0;
        UI.TestUI.textDefendC.visible = enemy.Defend > 0;
    }
}
#endregion