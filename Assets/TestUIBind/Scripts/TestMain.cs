using UnityEngine;

public class TestMain : MonoBehaviour
{
    void Start()
    {
        EnemyViewModel.Instance.Init(new EnemyM() { MaxHP = 100, CurHP = 100 });
    }
}