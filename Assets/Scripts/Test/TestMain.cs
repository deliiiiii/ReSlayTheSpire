using FairyGUI;
using UnityEngine;
public class TestMain : MonoBehaviour
{
    public WeaponClick weapon;
    public Coin coin;
    public Enemy enemy;


    Attack attack;

    
    OnClickRefine onClickRefine;
    OnClickHit onClickHit;

    void Start()
    {
        // 注入 UI 实例到 MyEvent 静态属性中
        MyEvent.TestUI = TestUI.Instance;

        weapon = new WeaponClick();
        coin = new Coin();
        enemy = new Enemy();

        attack = new Attack(weapon, enemy, coin);

        onClickRefine = new OnClickRefine(weapon, coin);
        onClickHit = new OnClickHit(weapon);
    }


    void Update()
    {
        attack.Update(Time.deltaTime);
    }
}