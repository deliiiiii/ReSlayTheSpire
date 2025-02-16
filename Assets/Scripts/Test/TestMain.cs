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
        testUI.Init();
        weapon = new WeaponClick(testUI);
        coin = new Coin(testUI);
        enemy = new Enemy(testUI);

        attack = new(weapon, enemy);
        onEnemyDie = new(attack, coin);


        onClickRefine = new(weapon, coin);
        onClickHit = new(weapon);
    }


    void Update()
    {
        attack.Update(Time.deltaTime);
    }
}