using System.Threading;
using UnityEngine;
public class Test : MonoBehaviour
{
    TimerM timer = new(5);
    AttackMediater attackMediater;
    void Start()
    {
        // TestUI.Instance.Init();
        attackMediater = new(new Weapon(20), new Enemy(), timer);
    }


    void Update()
    {
        timer.Update(Time.deltaTime);
        attackMediater.CallResult();
    }
}