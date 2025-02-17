public class Store
{
    Weapon weapon;
    CoinBag coinBag;
    public Store(Weapon weapon, CoinBag coinBag)
    {
        this.weapon = weapon;
        this.coinBag = coinBag;
        UI.TestUI.AddOnClickRefine(this);
        Cost = 10;
    }
    ~Store()
    {
        UI.TestUI.RemoveOnClickRefine();
    }
    int cost;

    public int Cost
    {
        get => cost;
        set
        {
            cost = value;
        }
    }

    public void UpdateUI()
    {
        UI.TestUI.buttonRefine.enabled = coinBag.Coin >= Cost;
        UI.TestUI.buttonRefine.text = $"Refine {Cost}";
    }
    public void OnClickRefine()
    {
        coinBag.UseCoin(Cost);
        weapon.Refine();
    }
}