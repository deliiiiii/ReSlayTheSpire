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
        TestUI.Instance.buttonRefine.onClick.Add(onClick);
    }
    ~OnClickRefine()
    {
        TestUI.Instance.buttonRefine.onClick.Remove(onClick);
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
        TestUI.Instance.buttonHit.onClick.Add(onClick);
    }
    ~OnClickHit()
    {
        TestUI.Instance.buttonHit.onClick.Remove(onClick);
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
    TestUI testUI;
    Weapon weapon;
    public OnWeaponChange(Weapon weapon)
    {
        this.testUI = TestUI.Instance;
        this.weapon = weapon;
    }
    public override void Fire()
    {
        testUI.textAttack.text = weapon.Damage.ToString();
    }
}



public class OnMoneyChange : MyEvent
{
    TestUI testUI;
    Coin coin;
    public OnMoneyChange(Coin coin)
    {
        this.testUI = TestUI.Instance;
        this.coin = coin;
    }
    public override void Fire()
    {
        testUI.textCoin.text = coin.Money.ToString();
        testUI.buttonRefine.enabled = coin.Money >= coin.Cost1;
        testUI.buttonRefine.text = $"Refine {coin.Cost1}";
    }
}

public class OnEnemyChange : MyEvent
{
    TestUI testUI;
    Enemy enemy;
    public OnEnemyChange(Enemy enemy)
    {
        this.testUI = TestUI.Instance;
        this.enemy = enemy;
    }
    public override void Fire()
    {
        testUI.textCurHP.text = enemy.Health.ToString();
        testUI.textMaxHP.text = enemy.MaxHP.ToString();
        testUI.textDefend.text = enemy.Defend.ToString();
        testUI.textDefend.visible = enemy.Defend > 0;
        testUI.textDefendC.visible = enemy.Defend > 0;
    }
}
#endregion
