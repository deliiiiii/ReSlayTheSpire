using System;

public abstract class Weapon
{
    OnWeaponChange onWeaponChange;
    public Weapon()
    {
        onWeaponChange = new OnWeaponChange(this);
        Damage = 10;
    }
    float damage;
    public float Damage
    {
        get => damage;
        set
        {
            damage = value;
            onWeaponChange.Fire();
        }
    }
    public abstract bool TryDoingAttack(float dt);
    public void Attack(float dt, Enemy enemy, Coin coin)
    {
        if (!TryDoingAttack(dt))
            return;
        if (enemy == null || !enemy.IsAlive())
            return;
        enemy.TakeDamage(Damage);
        if(!enemy.IsAlive())
        {
            enemy.DeadCount++;
            coin.Gain(enemy.GetReward());
            if(enemy.DeadCount % 3 == 0)
            {
                enemy.UpGrade();
            }
            enemy.Revive();
        }
    }
    public void Refine()
    {
        Damage = (float)Math.Round(Damage *= 1.1f, 1);
    }
}

public class WeaponClick : Weapon
{
    public bool clicked = false;
    public override bool TryDoingAttack(float dt)
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

    public override bool TryDoingAttack(float dt)
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