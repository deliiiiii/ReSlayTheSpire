using FairyGUI;
using UnityEngine;
public class TestMain : MonoBehaviour
{
    public WeaponClick weapon;
    public Coin coin;
    public Enemy enemy;

    OnClickRefine onClickRefine;
    OnClickHit onClickHit;

    void Start()
    {
        // 注入 UI 实例到 MyEvent 静态属性中
        UI.TestUI = TestUI.Instance;

        weapon = new WeaponClick();
        coin = new Coin();
        enemy = new Enemy();

        UI.TestUI.AddButtonOnClick(weapon, coin);
        
    }


    void Update()
    {
        weapon.Attack(Time.deltaTime, enemy, coin);
    }
}