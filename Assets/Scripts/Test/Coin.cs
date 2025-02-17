using System;

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