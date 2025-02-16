using FairyGUI;
using UnityEngine;
public class TestMain : MonoBehaviour
{
    public TestUI testUI;
    public WeaponClick weapon;
    public Coin coin;
    public Enemy enemy;


    Attack attack;
    OnEnemyDie onEnemyDie;

    
    OnClickRefine onClickRefine;
    OnClickHit onClickHit;

    void Start()
    {
        testUI = GameObject.Find("UIPanel").GetComponent<TestUI>();
        weapon = new WeaponClick();
        coin = new Coin();
        enemy = new Enemy();

        attack = new Attack(weapon, enemy);
        onEnemyDie = new OnEnemyDie(attack, coin);

        onClickRefine = new OnClickRefine(weapon, coin);
        onClickHit = new OnClickHit(weapon);

        attack.onEnemyDieEvent = () => onEnemyDie.Fire();
    }


    void Update()
    {
        attack.Update(Time.deltaTime);
    }
}