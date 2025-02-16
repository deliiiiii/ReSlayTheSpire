using UnityEngine;
public class Test : Singleton<Test>
{
    public TimerM timer;
    public Weapon weapon;
    public Coin coin;
    public Enemy enemy;
    AttackMediater attackMediater;
    void Start()
    {
        timer = new TimerM(5);
        weapon = new Weapon(TestUI.Instance);
        coin = new Coin(TestUI.Instance);
        enemy = new Enemy(TestUI.Instance);
        attackMediater = new();
    }


    void Update()
    {
        timer.Update(Time.deltaTime);
        attackMediater.CallResult();
    }
}