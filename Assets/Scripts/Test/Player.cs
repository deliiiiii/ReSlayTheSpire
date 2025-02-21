public interface IHasWeapon
{
    Weapon Weapon { get; }
    void UpdateAttack(float dt, Enemy enemy);
}
public class Player : IHasWeapon
{
    public Weapon Weapon { get; }
    public CoinBag CoinBag { get; }
    
    public Player()
    {
        Weapon = new WeaponClick();
        CoinBag = new CoinBag();
    }
    public void UpdateAttack(float dt, Enemy enemy)
    {
        if (!Weapon.TryDoingAttack(dt))
            return;
        if (enemy == null || !enemy.IsAlive())
            return;
        enemy.TakeDamage(Weapon.Damage);
        if (!enemy.IsAlive())
        {
            enemy.enemyManager.AddDeathCount();
            CoinBag.AddCoin(enemy.GetReward());
            if (enemy.enemyManager.DeathCount % 3 == 0)
            {
                enemy.UpGrade();
            }
            enemy.Revive();
        }
    }
    
}

public class CoinChangedEvent
{
    public int Coin;
}

public class CoinBag
{
    public CoinBag()
    {
        Coin = 0;
    }
    int coin;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            MyEvent.Fire(new CoinChangedEvent{Coin = coin});
        }
    }
    public void AddCoin(int add)
    {
        Coin += add;
    }
    public void UseCoin(int use)
    {
        Coin -= use;
    }
}
