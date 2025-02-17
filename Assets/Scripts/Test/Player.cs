public interface IHasWeapon
{
    Weapon Weapon { get; }
    void UpdateAttack(float dt, Enemy enemy);
}
// public interface IHasCoinBag
// {
//     CoinBag CoinBag { get; }
// }
public class Player : IHasWeapon//, IHasCoinBag
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
public class CoinBag
{
    public CoinBag()
    {
        onMoneyChange = new OnMoneyChange(this);
        Coin = 0;
    }
    int coin;
    public OnMoneyChange onMoneyChange;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            onMoneyChange.Fire();
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
