using FairyGUI;
using UnityEngine;
public class TestMain : MonoBehaviour
{
    public WeaponClick weapon;
    public Coin coin;
    public Enemy enemy;


    Attack attack;
    OnEnemyDie onEnemyDie;

    
    OnClickRefine onClickRefine;
    OnClickHit onClickHit;

    void Start()
    {
        // 注入 UI 实例到 MyEvent 静态属性中
        MyEvent.TestUI = TestUI.Instance;

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