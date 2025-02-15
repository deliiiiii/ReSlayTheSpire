using UnityEngine;
public class Test : MonoBehaviour
{
    AttackMediater attackMediater;
    void Start()
    {
        attackMediater = new(new Weapon(20), new Enemy());
    }


    void Update()
    {
        Timer.Update(Time.deltaTime);
        attackMediater.CallResult();
    }
}