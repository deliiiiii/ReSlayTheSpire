using System;
public class Attack : IUpdateMediate
{
    int deadCount = 0;
    Weapon weapon;
    Enemy enemy;
    Coin coin;

    public Attack(Weapon weapon, Enemy enemy, Coin coin)
    {
        this.weapon = weapon;
        this.enemy = enemy;
        this.coin = coin;
    }
    
    public void Update(float dt)
    {
        if (!weapon.IsDoingAttack(dt))
            return;
        if (!enemy.IsAlive())
            return;
        enemy.TakeDamage(weapon.Damage);
        if(!enemy.IsAlive())
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
}



