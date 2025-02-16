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
        
    }
}



