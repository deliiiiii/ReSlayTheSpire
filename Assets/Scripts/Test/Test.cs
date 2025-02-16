using UnityEngine;
public class Test : Singleton<Test>
{
    public TimerM timer = new(5);
    public Weapon weapon;
    public Coin coin;


    public Enemy enemy;
    Attack attack;
    OnEnemyDie onEnemyDie;

    void Start()
    {
        weapon = new Weapon(TestUI.Instance);
        coin = new Coin(TestUI.Instance);
        enemy = new Enemy(TestUI.Instance);
        attack = new(weapon, enemy);
        onEnemyDie = new(attack, coin);
    }


    void Update()
    {
        attack.Update();
    }
}