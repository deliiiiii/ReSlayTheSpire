using System;

public interface IWeaponUser
{
    Coin Coin { get; }
}

public abstract class Weapon
{
    IWeaponUser user;
    OnWeaponChange onWeaponChange;
    public Weapon(IWeaponUser user)
    {
        onWeaponChange = new OnWeaponChange(this);
        Damage = 10;
        this.user = user;
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
    public void Attack(float dt, Enemy enemy)
    {
        if (!TryDoingAttack(dt))
            return;
        if (enemy == null || !enemy.IsAlive())
            return;
        enemy.TakeDamage(Damage);
        if (!enemy.IsAlive())
        {
            enemy.enemyManager.IncrementDeathCount();
            user.Coin.Gain(enemy.GetReward());
            if (enemy.enemyManager.TotalDeathCount % 3 == 0)
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
    public WeaponClick(IWeaponUser user) : base(user)
    {
    }
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
    public WeaponAuto(IWeaponUser user) : base(user)
    {
    }
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