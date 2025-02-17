using System;

public class Player : IWeaponUser
{
    public Coin Coin { get; }
    WeaponClick weaponClick;
    public Player()
    {
        Coin = new Coin();
        weaponClick = new WeaponClick(this);
        UI.TestUI.AddButtonOnClick(weaponClick, Coin);
    }
    public void Update(float dt, Enemy enemy)
    {
        weaponClick.Attack(dt, enemy);
    }
}
public class Coin
{
    OnMoneyChange onMoneyChange;
    public Coin()
    {
        onMoneyChange = new OnMoneyChange(this);
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
            onMoneyChange.Fire();
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