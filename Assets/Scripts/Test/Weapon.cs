using System;

public struct WeaponData
{
    public float Damage;
}
public abstract class Weapon
{
    WeaponData weaponData;
    public Weapon()
    {
        weaponData = JsonIO.Read<WeaponData>("Weapon", "WeaponData");
        if (weaponData.Damage == 0)
        {
            weaponData.Damage = 10;
        }
    }
    public float Damage
    {
        get => weaponData.Damage;
        set
        {
            weaponData.Damage = value;
            // EventCentre.Instance.Fire<WeaponChangeEvent>(this);
            JsonIO.Write<WeaponData>("Weapon", "WeaponData", weaponData);
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