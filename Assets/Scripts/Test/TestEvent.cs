using System;
using FairyGUI;

public class OnEnemyDie : MyEvent
{
    public Attack attack;
    Weapon weapon => attack.weapon;
    Enemy enemy => attack.enemy;
    Coin coin;
    
    int deadCount = 0;
    public OnEnemyDie(Attack attack, Coin coin)
    {
        this.attack = attack;
        this.coin = coin;
    }
    public override void Fire()
    {
        deadCount++;
        MyDebug.Log($"killed:{deadCount}");
        coin.Gain(enemy.GetReward());
        if(deadCount % 3 == 0)
        {
            enemy.UpGrade();
        }
        enemy.Revive();
    }
}

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
        TestUI.buttonRefine.onClick.Add(onClick);
    }
    ~OnClickRefine()
    {
        TestUI.buttonRefine.onClick.Remove(onClick);
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
        TestUI.buttonHit.onClick.Add(onClick);
    }
    ~OnClickHit()
    {
        TestUI.buttonHit.onClick.Remove(onClick);
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
        TestUI.textAttack.text = weapon.Damage.ToString();
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
        TestUI.textCoin.text = coin.Money.ToString();
        TestUI.buttonRefine.enabled = coin.Money >= coin.Cost1;
        TestUI.buttonRefine.text = $"Refine {coin.Cost1}";
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
        TestUI.textCurHP.text = enemy.Health.ToString();
        TestUI.textMaxHP.text = enemy.MaxHP.ToString();
        TestUI.textDefend.text = enemy.Defend.ToString();
        TestUI.textDefend.visible = enemy.Defend > 0;
        TestUI.textDefendC.visible = enemy.Defend > 0;
    }
}
#endregion
