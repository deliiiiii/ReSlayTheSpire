using UnityEngine;
using FairyGUI;
using System;

public class Weapon
{
    public Weapon(TestUI testUI)
    {
        textAttack = testUI.component1.GetChild("textAttack").asCom.GetChild("text0").asTextField;
        testUI.component1.GetChild("textAttackC").asCom.GetChild("text0").text = "Attack";
        RefreshUI();
    }
    float damage = 10;
    GTextField textAttack;
    public bool IsDoingAttack()
    {
        return TestUI.Instance.ClickHit();
    }
    public float GetDamage()
    {
        return damage;
    }
    public void Refine()
    {
        damage *= 1.1f;
        damage = (float)System.Math.Round(damage, 1);
        RefreshUI();
    }
    public void RefreshUI()
    {
        textAttack.text = damage.ToString();
    }
}

public class Coin
{
    public Coin(TestUI testUI)
    {
        textCoin = testUI.component1.GetChild("textCoin").asCom.GetChild("text0").asTextField;
        textCoinC = testUI.component1.GetChild("textCoinC").asCom.GetChild("text0").asTextField;
        textCoinC.text = "Coin";
        buttonRefine = testUI.component1.GetChild("buttonRefine").asButton;
        SetCost(10);
    }
    int coin = 0;
    int cost = 0;
    GTextField textCoin;
    GTextField textCoinC;
    GButton buttonRefine;
    
    public void Gain(int add)
    {
        coin += add;
        RefreshUI();
    }
    public void Cost()
    {
        coin -= cost;
        SetCost(cost + 10);
    }
    public void SetCost(int cost)
    {
        this.cost = cost;
        RefreshUI();
    }
    public void RefreshUI()
    {
        textCoin.text = coin.ToString();
        buttonRefine.enabled = coin >= cost;
        buttonRefine.text = $"Refine {cost}";
    }
}

public class Enemy
{
    public Enemy(TestUI testUI)
    {
        textCurHP = testUI.component1.GetChild("textCurHP").asCom.GetChild("text0").asTextField;
        testUI.component1.GetChild("textHPSlash").asCom.GetChild("text0").text = "/";
        textMaxHP = testUI.component1.GetChild("textMaxHP").asCom.GetChild("text0").asTextField;
        textDefend = testUI.component1.GetChild("textDefend").asCom.GetChild("text0").asTextField;
        textDefendC = testUI.component1.GetChild("textDefendC").asCom.GetChild("text0").asTextField;
        textDefendC.text = "Defend";
        MaxHP = 100;
        Defend = 0;
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
            value = (float)System.Math.Round(value, 1);
            maxHP = value;
            RefreshHPUI();
        }
    }
    public float Health
    {
        get => health;
        set
        {
            value = (float)System.Math.Round(value, 1);
            health = value;
            if(health <= 0)
            {
                health = 0;
            }
            RefreshHPUI();
        }
    }
    public float Defend
    {
        get => defend;
        set
        {
            value = (float)System.Math.Round(value, 1);
            defend = value;
            RefreshDefendUI();
        }
    }

    GTextField textCurHP;
    GTextField textMaxHP;
    GTextField textDefend;
    GTextField textDefendC;


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
    public void RefreshDefendUI()
    {
        textDefend.text = Defend.ToString();
        textDefend.visible = Defend >= 0;
        textDefendC.visible = Defend >= 0;
    }
    public void RefreshHPUI()
    {
        textCurHP.text = Health.ToString();
        textMaxHP.text = MaxHP.ToString();
    }
}

public class Attack : IUpdateTrigger
{
    public Weapon weapon;
    public Enemy enemy;
    public event Action onEnemyDie;
    public Attack(Weapon weapon, Enemy enemy)
    {
        this.weapon = weapon;
        this.enemy = enemy;
    }
    
    public void Update()
    {
        if (!weapon.IsDoingAttack())
            return;
        if (!enemy.IsAlive())
            return;
        enemy.TakeDamage(weapon.GetDamage());
        if(!enemy.IsAlive())
            onEnemyDie?.Invoke();
    }
}

public class OnEnemyDie : IEvent
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
        attack.onEnemyDie += Fire;
    }
    public void Fire()
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
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale *= 2f;
            MyDebug.LogError($"Time.timeScale:{Time.timeScale}");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale /= 2f;
            MyDebug.LogError($"Time.timeScale:{Time.timeScale}");
        }
    }
    public bool isTimeOut => timer >= time;
    public void Decrease()
    {
        timer -= time;
    }
    public void RefineTime()
    {
        time = time * 0.9f;
    }
}
#endregion
