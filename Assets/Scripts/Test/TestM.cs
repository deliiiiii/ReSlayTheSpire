using UnityEngine;
using FairyGUI;

public class Weapon : IMediateObject
{
    public Weapon(TestUI testUI)
    {
        textAttack = testUI.component1.GetChild("textAttack").asCom.GetChild("text0").asTextField;
        testUI.component1.GetChild("textAttackC").asCom.GetChild("text0").text = "Attack";
        RefreshUI();
    }
    float damage = 10;
    GTextField textAttack;
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

public class Coin : IMediateObject
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

public class Enemy : IMediateObject
{
    public Enemy(TestUI testUI)
    {
        textCurHP = testUI.component1.GetChild("textCurHP").asCom.GetChild("text0").asTextField;
        testUI.component1.GetChild("textHPSlash").asCom.GetChild("text0").text = "/";
        textMaxHP = testUI.component1.GetChild("textMaxHP").asCom.GetChild("text0").asTextField;
        textDefend = testUI.component1.GetChild("textDefend").asCom.GetChild("text0").asTextField;
        textDefendC = testUI.component1.GetChild("textDefendC").asCom.GetChild("text0").asTextField;
        textDefendC.text = "Defend";

        Revive();
    }
    float maxHP = 100;
    float defend = 0;
    float health;

    GTextField textCurHP;
    GTextField textMaxHP;
    GTextField textDefend;
    GTextField textDefendC;
    
    

    public bool IsAlive()
    {
        return health > 0;
    }
    public void TakeDamage(float damage)
    {
        float HP1 = health;
        float trueDamage = (damage - defend) < 0 ? damage/10f : (damage - defend);
        health = HP1 - trueDamage;
        health = (float)System.Math.Round(health, 1);
        MyDebug.Log($"{HP1} - {trueDamage} = {health}");
        RefreshHPUI();
    }
    public void Revive()
    {
        MyDebug.Log($"Revive! {maxHP}");
        health = maxHP;
        RefreshHPUI();
        RefreshDefendUI();
    }
    public void UpGrade()
    {
        maxHP += 10;
        defend += 1;
        MyDebug.Log($"UpGrade! maxHP:{maxHP} defense:{defend}");
    }

    public int GetReward()
    {
        return (int)(maxHP/20) + (int)defend;
    }
    public void RefreshDefendUI()
    {
        textDefend.text = defend.ToString();
        textDefend.visible = defend >= 0;
        textDefendC.visible = defend >= 0;
    }
    public void RefreshHPUI()
    {
        textCurHP.text = health.ToString();
        textMaxHP.text = maxHP.ToString();
    }
}

public static class AttackPriority
{
    public static float INPUT = float.MinValue;
    public static float ENEMYCHECK = 0;
    public static float DAMAGE = 1;
    public static float UI = 5;
    public static float EVENT = 10;
}
public class AttackM : Mediater
{
    Mediater enemyKilledM;
    Mediater enemyDead10M;
    Mediater decTimerM;
    
    public AttackM() : base()
    {
        Enemy enemy = Test.Instance.enemy;
        Weapon weapon = Test.Instance.weapon;
        Coin coin = Test.Instance.coin;
        TimerM timer = Test.Instance.timer;
        

        enemyKilledM = new();
        enemyDead10M = new();
        decTimerM = new();


        AddCheck(() => TestUI.Instance.ClickHit(), AttackPriority.INPUT);
        AddCheck(() => timer.isTimeOut, AttackPriority.INPUT);
        AddCheck(() => enemy.IsAlive(), AttackPriority.ENEMYCHECK);
        
        AddTrueCB(()=>enemy.TakeDamage(weapon.GetDamage()), AttackPriority.DAMAGE);
        AddTrueCB(enemyKilledM, AttackPriority.EVENT);
            enemyKilledM.AddCheck(() => !enemy.IsAlive(), 0);
            enemyKilledM.AddTrueCB(() =>{deadCount++; MyDebug.Log($"killed:{deadCount}");}, 0);
            enemyKilledM.AddTrueCB(()=>coin.Gain(enemy.GetReward()), 0);
            enemyKilledM.AddTrueCB(enemyDead10M, 1);
                enemyDead10M.AddCheck(() => deadCount % 3 == 0, 0);
                enemyDead10M.AddTrueCB(() => {enemy.UpGrade();}, 0);
            enemyKilledM.AddTrueCB(() => enemy.Revive(), 1);
        AddTrueCB(decTimerM, AttackPriority.EVENT + 1);
            decTimerM.AddCheck(() => timer.isTimeOut, 0);
            decTimerM.AddTrueCB(()=>MyDebug.Log("↑ Triggered!"), 0);
            decTimerM.AddTrueCB(()=>timer.Decrease(), 0);
    }
    int deadCount = 0;
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
