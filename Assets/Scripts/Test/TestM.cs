using System;
using UnityEngine;

public class Weapon : IMediateObject
{
    public Weapon(float damage)
    {
        Damage = damage;
    }
    float damage;
    public float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
}

public class Enemy : IMediateObject
{
    public Enemy()
    {
        health = maxHP;
    }
    float maxHP = 100;
    float defense = 0;
    float health;
    public bool IsAlive()
    {
        return health > 0;
    }
    public void TakeDamage(float damage)
    {
        float HP1 = health;
        float trueDamage = (damage - defense) < 0 ? damage/10f : (damage - defense);
        health = HP1 - trueDamage;
        MyDebug.Log($"{HP1} - {trueDamage} = {health}");
    }
    public void Revive()
    {
        MyDebug.Log($"Revive! {maxHP}");
        health = maxHP;
    }
    public void UpGrade()
    {
        maxHP += 10;
        defense += 1;
        MyDebug.Log($"UpGrade! maxHP:{maxHP} defense:{defense}");
    }
}

public static class AttackPriority
{
    public static float INPUT = float.MinValue;
    public static float ENEMYCHECK = 0;
    public static float DAMAGE = 1;
    public static float EVENT = 10;
}
public class AttackMediater : Mediater
{
    public AttackMediater(Weapon weapon, Enemy enemy) : base()
    {
        this.weapon = weapon;
        this.enemy = enemy;
        AddCheck(() => InputManager.isAttackKeyDown, AttackPriority.INPUT);
        AddCheck(() => Timer.isTriggered, AttackPriority.INPUT);
        AddCheck(() => enemy.IsAlive(), AttackPriority.ENEMYCHECK);
        AddTrueCB(
            ()=>
            {
                enemy.TakeDamage(weapon.Damage);
            }, AttackPriority.DAMAGE);
        
        EnemyTotalKilledEvent enemyTotalKilledEvent = new(this);
        AddTrueCB(enemyTotalKilledEvent, AttackPriority.EVENT);
    }
    public Weapon weapon;
    public Enemy enemy;
}

public class EnemyTotalKilledEvent : MyEvent
{
    int killed = 0;
    public EnemyTotalKilledEvent(AttackMediater attackMediater) : base(attackMediater)
    {
        this.attackMediater = attackMediater;
        AddCheck(() => !attackMediater.enemy.IsAlive(), 0);
        AddTrueCB(() =>{killed++; MyDebug.Log($"killed:{killed}");}, -1);
        EnemyUpgradeEvent enemyUpgradeEvent = new(this);
        AddTrueCB(enemyUpgradeEvent, 0);
        AddTrueCB(() => attackMediater.enemy.Revive(), 1);
    }
    public AttackMediater attackMediater;
    public int GetKilled()
    {
        return killed;
    }
    public void Reset()
    {
        killed = 0;
    }
}

public class EnemyUpgradeEvent : MyEvent
{
    public EnemyUpgradeEvent(EnemyTotalKilledEvent enemyTotalKilledEvent) : base(enemyTotalKilledEvent)
    {
        AddCheck(() => enemyTotalKilledEvent.GetKilled() >= 10, 0);
        AddTrueCB(() => {enemyTotalKilledEvent.Reset();enemyTotalKilledEvent.attackMediater.enemy.UpGrade();}, 0);
    }
}

#region Input
public class InputManager
{
    public static bool isAttackKeyDown => Input.GetKeyDown(KeyCode.A);
}

public static class Timer
{
    public static float timer = 0;
    public static float time = 2;
    public static bool isTriggered = false;
    public static void Update(float dt)
    {
        timer += dt;
        if (timer >= time)
        {
            timer -= time;
            MyDebug.Log("trigger!");
            isTriggered = true;
            return;
        }
        isTriggered = false;
    }
}
#endregion
