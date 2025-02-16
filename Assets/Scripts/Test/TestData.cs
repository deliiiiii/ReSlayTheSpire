using FairyGUI;
using System;
using UnityEngine;

public abstract class Weapon
{
    OnWeaponChange onWeaponChange;
    public Weapon()
    {
        onWeaponChange = new(this);
        Damage = 10;
    }
    float damage;
    public float Damage
    {
        get => damage;
        set
        {
            damage = value;
            MyEventSystem.Fire<OnWeaponChange>();
        }
    }
    public abstract bool IsDoingAttack(float dt);
    public void Refine()
    {
        Damage = (float)Math.Round(Damage *= 1.1f, 1);
    }
}


public class WeaponClick : Weapon
{
    public bool clicked = false;
    public override bool IsDoingAttack(float dt)
    {
        bool ret = clicked;
        if(clicked)
            clicked = false;
        return ret;
    }
}

public class WeaponAuto : Weapon
{
    float timer = 0;
    readonly float time = 1;

    public override bool IsDoingAttack(float dt)
    {
        timer += dt;
        if(timer >= time)
        {
            timer -= time;
            return true;
        }
        return false;
    }
}

public class Coin
{
    OnMoneyChange onMoneyChange;
    public Coin()
    {
        onMoneyChange = new(this);
        cost = 10;
        Money = 0;
    }
    int money;
    int cost;

    public int Money
    {
        get => money;
        set
        {
            money = value;
            MyEventSystem.Fire<OnMoneyChange>();
        }
    }

    public int Cost1 { get => cost; set => cost = value; }

    public void Gain(int add)
    {
        Money += add;
    }
    public void Cost()
    {
        Money -= Cost1;
        Cost1 += 10;
    }
}

public class Enemy
{
    OnEnemyChange onEnemyChange;
    public Enemy()
    {
        onEnemyChange = new(this);
        maxHP = 100;
        defend = 0;
        Health = MaxHP;
    }
    float maxHP;
    float health;
    float defend;
    public float MaxHP 
    { 
        get => maxHP;
        set
        {
            value = (float)Math.Round(value, 1);
            maxHP = value;
            MyEventSystem.Fire<OnEnemyChange>();
        }
    }
    public float Health
    {
        get => health;
        set
        {
            value = (float)Math.Round(value, 1);
            health = value;
            if(health <= 0)
            {
                health = 0;
            }
            MyEventSystem.Fire<OnEnemyChange>();
        }
    }
    public float Defend
    {
        get => defend;
        set
        {
            value = (float)Math.Round(value, 1);
            defend = value;
            MyEventSystem.Fire<OnEnemyChange>();
        }
    }

   


    public bool IsAlive()
    {
        return Health > 0;
    }
    public void TakeDamage(float damage)
    {
        float HP1 = Health;
        float trueDamage = (damage - Defend) < 0 ? damage/10f : (damage - Defend);
        Health = HP1 - trueDamage;
        MyDebug.Log($"{HP1} - {trueDamage} = {Health}");
    }
    public void Revive()
    {
        Health = MaxHP;
        MyDebug.Log($"Revive! {MaxHP}");
    }
    public void UpGrade()
    {
        MaxHP += 10;
        Defend += 1;
        MyDebug.Log($"UpGrade! maxHP:{MaxHP} defense:{Defend}");
    }

    public int GetReward()
    {
        return (int)(MaxHP/20) + (int)Defend;
    }
}



