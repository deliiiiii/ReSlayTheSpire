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
    Coin coin;
    public OnMoneyChange(Coin coin)
    {
        this.coin = coin;
    }
    public void Fire()
    {
        UI.TestUI.textCoin.text = coin.Money.ToString();
        UI.TestUI.buttonRefine.enabled = coin.Money >= coin.Cost1;
        UI.TestUI.buttonRefine.text = $"Refine {coin.Cost1}";
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

public class OnEnemyDie : IEvent
{
    Enemy enemy;
    public OnEnemyDie(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public void Fire()
    {
    }
}
