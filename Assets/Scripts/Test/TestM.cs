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
        
        AddTrueCB(new EnemyKilledEvent(this), AttackPriority.EVENT);
    }
    public Weapon weapon;
    public Enemy enemy;
}

public class EnemyKilledEvent : MyEvent
{
    public EnemyKilledEvent(AttackMediater attackMediater) : base(attackMediater)
    {
        weapon = attackMediater.weapon;
        enemy = attackMediater.enemy;
        AddCheck(() => !enemy.IsAlive(), 0);
        AddTrueCB(new EnemyDeadCountMediater(this), 0);
        AddTrueCB(() => enemy.Revive(), 1);
    }
    public Weapon weapon;
    public Enemy enemy;
}

public class EnemyDeadCountMediater : Mediater
{
    int deadCount = 0;
    public EnemyDeadCountMediater(EnemyKilledEvent enemyKilledEvent) : base()
    {
        weapon = enemyKilledEvent.weapon;
        enemy = enemyKilledEvent.enemy;
        AddTrueCB(() =>{deadCount++; MyDebug.Log($"killed:{deadCount}");}, 0);
        AddTrueCB(new Dead10Mediater(this), 1);
    }
    public Weapon weapon;
    public Enemy enemy;
    public int GetDeadCount()
    {
        return deadCount;
    }
    public void Reset()
    {
        deadCount = 0;
    }
}

public class Dead10Mediater : Mediater
{
    public Dead10Mediater(EnemyDeadCountMediater enemyTotalKilledEvent) : base()
    {
        weapon = enemyTotalKilledEvent.weapon;
        enemy = enemyTotalKilledEvent.enemy;
        AddCheck(() => enemyTotalKilledEvent.GetDeadCount() >= 10, 0);
        AddTrueCB(() => {enemyTotalKilledEvent.Reset();enemy.UpGrade();}, 0);
    }
    public Weapon weapon;
    public Enemy enemy;
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
