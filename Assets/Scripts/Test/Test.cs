using System.Threading;
using UnityEngine;
public class Test : MonoBehaviour
{
    TimerM timer;
    AttackMediater attackMediater;
    Weapon weapon;
    Enemy enemy;
    void Start()
    {
        timer = new TimerM(5);

        weapon = new Weapon();
        enemy = new Enemy(UIEnemy.Instance);
        attackMediater = new(weapon, enemy, timer);
    }


    void Update()
    {
        timer.Update(Time.deltaTime);
        attackMediater.CallResult();
    }
}