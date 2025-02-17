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
    public void Refine()
    {
        Damage = (float)Math.Round(Damage *= 1.1f, 1);
    }
}

public class WeaponClick : Weapon
{
    public WeaponClick() : base()
    {
        UI.TestUI.AddOnClickHit(this);
    }
    ~WeaponClick()
    {
        UI.TestUI.RemoveOnClickHit();
    }
    public bool clicked = false;
    public override bool TryDoingAttack(float dt)
    {
        bool ret = clicked;
        if(clicked)
            clicked = false;
        return ret;
    }
    public void OnClickHit()
    {
        clicked = true;
    }
}

public class WeaponAuto : Weapon
{
    public WeaponAuto() : base()
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