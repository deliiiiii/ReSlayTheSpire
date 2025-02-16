using System;
using System.Threading;
using UnityEngine;

public class Weapon : IMediateObject
{
    public Weapon(float damage)
    {
        this.damage = damage;
    }
    float damage;
    public float GetDamage()
    {
        return damage;
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
    Mediater enemyKilledM;
    Mediater enemyDead10M;
    Mediater decTimerM;
    
    public AttackMediater(Weapon weapon, Enemy enemy, TimerM timer) : base()
    {
        this.weapon = weapon;
        this.enemy = enemy;

        enemyKilledM = new();
        enemyDead10M = new();
        decTimerM = new();


        AddCheck(() => InputManager.isAttackKeyDown, AttackPriority.INPUT);
        AddCheck(() => timer.isTriggered, AttackPriority.INPUT);
        AddCheck(() => enemy.IsAlive(), AttackPriority.ENEMYCHECK);
        
        AddTrueCB(()=>enemy.TakeDamage(weapon.GetDamage()), AttackPriority.DAMAGE);
        AddTrueCB(enemyKilledM, AttackPriority.EVENT);
            enemyKilledM.AddCheck(() => !enemy.IsAlive(), 0);
            enemyKilledM.AddTrueCB(() =>{deadCount++; MyDebug.Log($"killed:{deadCount}");}, 0);
            enemyKilledM.AddTrueCB(enemyDead10M, 1);
                enemyDead10M.AddCheck(() => deadCount >= 10, 0);
                enemyDead10M.AddTrueCB(() => {Reset();enemy.UpGrade();}, 0);
            enemyKilledM.AddTrueCB(() => enemy.Revive(), 1);
        AddTrueCB(decTimerM, AttackPriority.EVENT + 1);
            decTimerM.AddCheck(() => timer.isTriggered, 0);
            decTimerM.AddTrueCB(()=>MyDebug.Log("Triggered!"), 0);
            decTimerM.AddTrueCB(()=>timer.Decrease(), 0);
    }
    public Weapon weapon;
    public Enemy enemy;

    int deadCount = 0;
    public void Reset()
    {
        deadCount = 0;
    }
}

#region Input
public class InputManager
{
    public static bool isAttackKeyDown => Input.GetKeyDown(KeyCode.A);
}

public class TimerM
{
    public TimerM(float f_time)
    {
        time = f_time;
        timer = 0;
    }
    public float timer = 0;
    public float time;

    public void Update(float dt)
    {
        timer += dt;
    }
    public bool isTriggered => timer >= time;
    public void Decrease()
    {
        timer -= time;
    }
}
#endregion
