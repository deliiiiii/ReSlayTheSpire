using System;
public class Attack : IUpdateMediate
{
    public Weapon weapon;
    public Enemy enemy;
    public Attack(Weapon weapon, Enemy enemy)
    {
        this.weapon = weapon;
        this.enemy = enemy;
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
            MyEventSystem.Fire<OnEnemyDie>();
        }
    }
}



