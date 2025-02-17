using System;

public class Enemy
{
    public EnemyManager enemyManager;
    OnEnemyChange onEnemyChange;
    public Enemy(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
        onEnemyChange = new OnEnemyChange(this);
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
            onEnemyChange.Fire();
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
            onEnemyChange.Fire();
        }
    }
    public float Defend
    {
        get => defend;
        set
        {
            value = (float)Math.Round(value, 1);
            defend = value;
            onEnemyChange.Fire();
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }
    public void TakeDamage(float damage)
    {
        float HP1 = Health;
        float trueDamage = (damage - Defend) < 0 ? damage / 10f : (damage - Defend);
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