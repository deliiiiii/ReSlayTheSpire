public class Store
{
    public Weapon Weapon { get; }
    public CoinBag CoinBag { get; }
    public Store(Weapon weapon, CoinBag coinBag)
    {
        Weapon = weapon;
        CoinBag = coinBag;
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
        UI.TestUI.buttonRefine.enabled = CoinBag.Coin >= Cost;
        UI.TestUI.buttonRefine.text = $"Refine {Cost}";
    }
    public void OnClickRefine()
    {
        CoinBag.UseCoin(Cost);
        Weapon.Refine();
    }
}