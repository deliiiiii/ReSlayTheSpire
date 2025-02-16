using System;
using System.Collections.Generic;

public static class MyEventSystem
{
    public static void Fire<T>() where T : MyEvent
    {
        eventDic[typeof(T).Name].Fire();
    }
    public static Dictionary<string,MyEvent> eventDic = new();
}

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

public class OnClickRefine : MyEvent
{
    Weapon weapon;
    Coin coin;
    public OnClickRefine(Weapon weapon, Coin coin)
    {
        this.weapon = weapon;
        this.coin = coin;
    }
    public override void Fire()
    {
        coin.Cost();
        weapon.Refine();
    }
}

public class OnWeaponChange : MyEvent
{
    TestUI testUI;
    Weapon weapon;
    public OnWeaponChange(TestUI testUI, Weapon weapon)
    {
        this.testUI = testUI;
        this.weapon = weapon;
    }
    public override void Fire()
    {
        testUI.textAttack.text = weapon.Damage.ToString();
    }
}

public class OnClickHit : MyEvent
{
    WeaponClick weaponClick;
    public OnClickHit(WeaponClick weaponClick)
    {
        this.weaponClick = weaponClick;
    }
    public override void Fire()
    {
        weaponClick.clicked = true;
    }
}

public class OnMoneyChange : MyEvent
{
    TestUI testUI;
    Coin coin;
    public OnMoneyChange(TestUI testUI, Coin coin)
    {
        this.testUI = testUI;
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
    public OnEnemyChange(TestUI testUI, Enemy enemy)
    {
        this.testUI = testUI;
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